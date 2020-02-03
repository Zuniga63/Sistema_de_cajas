using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Control_de_cajas.Modelo
{
    class BDComun
    {
        public static string Server { get; set; }
        public static string UsserID { get; set; }
        public static string Password { get; set; }
        public static string DataBase { get; set; }
        public static string Error { get; set; }

        private static MySqlConnection conexionDB;
        private static MySqlDataReader reader;

        

        /// <summary>
        /// Crea una conexion estable con la base de datos, pero antes cierra la conexion si está
        /// se encuentra abierta.
        /// </summary>
        /// <returns>True, si la conexion se establecio correctamente</returns>
        private static bool ObtenerConexion()
        {
            //Cierro la conexion si se encuentra abierta
            if (conexionDB != null)
            {
                if (conexionDB.State == System.Data.ConnectionState.Open)
                {
                    conexionDB.Close();
                }
            }

            //Creo la cadena de conexion
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = Server,
                UserID = UsserID,
                Password = Password,
                Database = DataBase,
                AllowUserVariables = true
            };

            conexionDB = new MySqlConnection(builder.ToString());

            try
            {
                conexionDB.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                if (ex.InnerException != null)
                {
                    CrearMensaje(ex.Number, ex.InnerException.Message);
                }
                else
                {
                    CrearMensaje(ex.Number, ex.Message);
                }
                
                return false;
            }

        }

        /// <summary>
        /// Este metodo busca en el listado de errores y reescribe el mensaje de error
        /// si este no se encuentra por defecto muestra el codigo de error y el mensaje de la consola.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        private static void CrearMensaje(int number, string message)
        {
            switch (number)
            {
                case 1042:
                    Error = string.Format("No se puede conectar al host especificado: {0}", Server);
                    break;
                default:
                    Error = string.Format("Codigo de error {0}: {1}", number, message);
                    break;
            }
        }

        public static bool VerificarConexion()
        {
            if (ObtenerConexion())
            {
                conexionDB.Close();             //Cierro la conexion
                return true;
            }

            return false;
        }

        private static bool HacerConsulta(string query)
        {
            reader = null;
            MySqlCommand cmd;

            if (ObtenerConexion())
            {
                try
                {
                    cmd = new MySqlCommand(query, conexionDB);
                    reader = cmd.ExecuteReader();
                    return true;
                }
                catch (MySqlException ex)
                {
                    CrearMensaje(ex.Number, ex.Message);
                }

            }

            return false;
        }

        public static bool ModificarTabla(string query)
        {
            try
            {
                if (ObtenerConexion())
                {
                    MySqlCommand cmd = new MySqlCommand(query, conexionDB);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        return true;
                    }

                    else
                    {
                        return false;
                    }

                }
            }
            catch (MySqlException ex)
            {
                CrearMensaje(ex.Number, ex.Message);
                return false;
            }
            finally
            {
                conexionDB.Close();
            }

            return false;
        }

        #region INGRESO Y CONSULTAS DE USUARIOS 
        /// <summary>
        /// Este metodo crea un nuevo usuario en la base de datos
        /// </summary>
        /// <param name="userName">EL nombre del usuario</param>
        /// <returns></returns>
        public static bool NewUser(string userName)
        {
            bool result = false;
            string createDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            createDate = PonerComillas(createDate);

            userName = PonerComillas(userName);
            string query = "INSERT INTO user (user_name, create_date) VALUES ";
            query += string.Format("({0}, {1:yyyy-MM-dd hh:mm:ss})", userName, createDate);

            if(ModificarTabla(query))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Este metodo elimina de la base de datos la informacion del usuario y las categorias asociadas a este
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool DeleteUser(int userId)
        {
            if(DeleteCategoryOfUser(userId))
            {
                string query = "DELETE FROM user WHERE user_id = " + userId;
                return ModificarTabla(query);
            }

            return false;
        }

        /// <summary>
        /// List<Category> categories = ReadAllCategories(userId);
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static bool DeleteCategoryOfUser(int userId)
        {

            if(VerificarConexion())
            {
                List<Category> categories = ReadAllCategories(userId);

                if (categories.Count > 0)
                {
                    string query = "START TRANSACTION; ";

                    foreach (Category c in categories)
                    {
                        query += string.Format("DELETE FROM category WHERE category_id = {0}; ", c.ID);
                    }

                    query += "COMMIT";
                    return ModificarTabla(query);
                }

                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Este metodo actualiza los campos del usaurio y hace tres consultas a la base de datos. Para recuperar el nombre
        /// para recuperar el numero de cajas y sus saldos, y una tercera para recuperar el numero de clientes y sus saldos
        /// </summary>
        /// <param name="user">Instancia de usuario</param>
        /// <returns></returns>
        public static bool ReloadUser(User user)
        {
            if(user == null)
            {
                return false;
            }

            string userName = null;
            int cashBoxs = 0;
            int customers = 0;
            decimal cashBalances = 0m;
            decimal customerBalances = 0m;

            //Primero actualizo los datos del suaurio
            string query1 = "SELECT user_name FROM user WHERE user_id = " + user.ID;        //Para actualizar el nombre de usuario
            string query2 = "SELECT balance FROM cashbox WHERE user_id = " + user.ID;       //Para actualizar el numero de cajas y el saldo
            string query3 = "SELECT balance FROM customer WHERE user_id = " + user.ID;      //Para actualizar el numero de clientes y el saldo

            if(HacerConsulta(query1))
            {
                if(reader.Read())
                {
                    userName = reader.GetString("user_name");
                }

                //Ahora actualizo los datos de las cajas
                if (HacerConsulta(query2))
                {
                    while(reader.Read())
                    {
                        cashBoxs++;
                        cashBalances += reader.GetDecimal("balance");
                    }

                    //Ahora se consulta los saldos de los clientes
                    if(HacerConsulta(query3))
                    {
                        while(reader.Read())
                        {
                            customers++;
                            customerBalances += reader.GetDecimal("balance");
                        }

                        //Finalmente se actualiza el usuario y se retorna true
                        user.UserName = userName;
                        user.Boxs = cashBoxs;
                        user.CashBalances = cashBalances;
                        user.Customers = customers;
                        user.CustomerBalances = customerBalances;
                        return true;
                    }//Fin del tercer if
                    else
                    {
                        return false;
                    }
                }//Fin del segundo if
                else
                {
                    return false;
                }
            }//Fin del primer if
            else//En caso de que no se pueda recuperar el nombre del usuario
            {
                return false;
            }
        }

        /// <summary>
        /// Recupera la informacion de todos los usuario de la base de datos y para cada usuario
        /// hace las consultas correspondientes a los saldos de las cajas y los clientes
        /// </summary>
        /// <returns></returns>
        public static List<User> ReadAllUser()
        {
            List<User> allUsers = new List<User>();                 //Para guardar las instancias de los usuarios;
            string query = "SELECT * FROM user";

            //Primero se recupera los datos de los usuarios
            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("user_id");
                    string userName = reader.GetString("user_name");
                    DateTime createDate = reader.GetDateTime("create_date");

                    User u = new User(id, userName, createDate);
                    allUsers.Add(u);
                }
            }

            //Se actualiza la informacion de los usuario, si alguna consulta falla entonces
            //se cancela toda la operacion
            foreach(User user in allUsers)
            {
                if(!ReloadUser(user))
                {
                    allUsers.Clear();
                    break;
                }
            }

            return allUsers;

        }

        public static bool CreateCasbox(int userID, string name)
        {
            name = PonerComillas(name);

            string query = "INSERT INTO cashbox(user_id, box_name) VALUES ";
            query += "( " + userID + ", " + name + ")";

            return ModificarTabla(query);
        }

        public static List<Cashbox> ReadAllCashbox(int userId)
        {
            List<Cashbox> result = new List<Cashbox>();
            string query = "SELECT * FROM cashbox WHERE user_id = " + userId;

            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("cashbox_id");
                    string name = reader.GetString("box_name");
                    decimal balance = reader.GetDecimal("balance");

                    Cashbox cBox = new Cashbox(id, name, balance);
                    result.Add(cBox);
                }
            }

            return result;
        }

        public static bool ReloadCashbox(Cashbox cashbox)
        {
            string query = "SELECT box_name, balance FROM cashbox WHERE cashbox_id = " + cashbox.ID;
            query += " LIMIT 1";

            if(HacerConsulta(query))
            {
                reader.Read();
                cashbox.BoxName = reader.GetString("box_name");
                cashbox.Balance = reader.GetDecimal("balance");
                return true;
            }

            return false;
        }

        public static bool AddNewTransaction(int userId, int cashboxId, DateTime transactionDate, string description,
            decimal amount, List<int> categories)
        {
            description = PonerComillas(description);
            string date = PonerComillas(transactionDate.ToString("yyyy-MM-dd"));

            string query = "START TRANSACTION; ";
            query += "INSERT INTO transacction(cashbox_id, user_id, transacction_date, description, amount) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4}); ", cashboxId, userId, date, description, amount);
            query += "SET @last_transaction_id = LAST_INSERT_ID(); ";
            query += "INSERT INTO transacction_has_category (transacction_id, cashbox_id, user_id, category_id) VALUES";

            foreach(int categoryId in categories)
            {
                query += string.Format(" (@last_transaction_id, {0}, {1}, {2}),", cashboxId, userId, categoryId);
            }

            query = query.TrimEnd(',') + "; ";

            query += string.Format("UPDATE cashbox SET balance = balance + {0} WHERE cashbox_id = {1}; ", amount, cashboxId);
            query += "COMMIT";

            return ModificarTabla(query);
        }

        public static List<CashboxTransaction> ReadAllTransactions(int user_id, int cashbox_id)
        {
            List<CashboxTransaction> result = new List<CashboxTransaction>();
            string query = "SELECT transacction_id, transacction_date, description, amount, transfer FROM transacction WHERE ";
            query += string.Format("user_id = {0} AND cashbox_id = {1} ORDER BY transacction_date, amount DESC", user_id, cashbox_id);

            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("transacction_id");
                    DateTime tDate = reader.GetDateTime("transacction_date");
                    string description = reader.GetString("description");
                    decimal amount = reader.GetDecimal("amount");
                    bool isATransfer = reader.GetBoolean("transfer");
                    result.Add(new CashboxTransaction(id, tDate, description, amount, isATransfer));
                }
            }

            return result;
        }

        public static bool UpdateTransaction(CashboxTransaction originalTransaction, DateTime date, string description, decimal amount)
        {
            string query = "START TRANSACTION; ";
            query += "SET @box_id = (SELECT cashbox_id FROM transacction WHERE transacction_id = " + originalTransaction.ID +"); ";
            query += string.Format("UPDATE cashbox SET balance = balance + {0} WHERE cashbox_id = @box_id; ", originalTransaction.Amount * -1);
            query += "UPDATE transacction SET ";
            query += "transacction_date = " + PonerComillas(date.ToString("yyyy-MM-dd")) + ", ";
            query += "description = " + PonerComillas(description) + ", ";
            query += "amount = " + amount + " ";
            query += "WHERE transacction_id = " + originalTransaction.ID + "; ";
            query += string.Format("UPDATE cashbox SET balance = balance + {0} WHERE cashbox_id = @box_id; ", amount);
            query += "COMMIT";

            return ModificarTabla(query);
        }

        public static CashboxTransaction RecuperarUltimaTransaccion(int user_id, int cashbox_id)
        {
            CashboxTransaction transaction = null;
            string query = "SELECT transacction_id, transacction_date, description, amount, transfer FROM transacction WHERE ";
            query += string.Format("user_id = {0} AND cashbox_id = {1} ", user_id, cashbox_id);
            query += "ORDER BY transacction_id DESC LIMIT 1";

            if (HacerConsulta(query))
            {
                reader.Read();
                int id = reader.GetInt32("transacction_id");
                DateTime tDate = reader.GetDateTime("transacction_date");
                string description = reader.GetString("description");
                decimal amount = reader.GetDecimal("amount");
                bool isATransfer = reader.GetBoolean("transfer");
                transaction = new CashboxTransaction(id, tDate, description, amount, isATransfer);
            }

            return transaction;
        }

        public static bool MakeTransfer(int userId, int senderBoxID, int adresseBox, decimal amountToTransfer)
        {
            DateTime now = DateTime.Now;
            string currentDate = PonerNullOrComillas(now.ToString("yyyy-MM-dd"));
            string descripcion = string.Format("Transferencia en efectivo {0}-{1}-{2}-{3:yyyyMMddHHmmss}", userId, senderBoxID, adresseBox, now);
            string hash = string.Format("{0}-{1}-{2}-{3:yyyyMMddHHmmss}", userId, senderBoxID, adresseBox, now);

            descripcion = PonerComillas(descripcion);
            hash = PonerComillas(hash);

            string query = "START TRANSACTION; ";
            query += "INSERT INTO transfer(cashbox_sender_id, user_sender_id, cashbox_adressed_id, user_adressed_id, transfer_date, amount_transferred, hash) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}); ", senderBoxID, userId, adresseBox, userId, currentDate, amountToTransfer, hash);
            query += "INSERT INTO transacction(cashbox_id, user_id, transacction_date, description, amount, transfer) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4}, TRUE), ", senderBoxID, userId, currentDate, descripcion, -amountToTransfer);
            query += string.Format("({0}, {1}, {2}, {3}, {4}, TRUE); ", adresseBox, userId, currentDate, descripcion, amountToTransfer);
            query += string.Format("UPDATE cashbox SET balance = balance - {0} WHERE cashbox_id = {1}; ", amountToTransfer, senderBoxID);
            query += string.Format("UPDATE cashbox SET balance = balance + {0} WHERE cashbox_id = {1}; ", amountToTransfer, adresseBox);
            query += string.Format("COMMIT;");

            return ModificarTabla(query);

        }

        #endregion

        #region CLIENTES
        /// <summary>
        /// Consulta en la base de datos la informacion de los clientes del usuario
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static List<Customer> ReadAllCustomers(User user)
        {
            List<Customer> customers = new List<Customer>();
            string query = "SELECT customer_id, customer_name, balance, observation, nit, address, phone FROM customer ";
            query += "WHERE user_id = " + user.ID;

            
            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("customer_id");
                    string name = reader.GetString("customer_name");
                    decimal balance = reader.GetDecimal("balance");
                    string observation = reader.IsDBNull(3) ? null : reader.GetString("observation");
                    string nit = reader.IsDBNull(4) ? null : reader.GetString("nit");
                    string address = reader.IsDBNull(5) ? null : reader.GetString("address");
                    string phone = reader.IsDBNull(6) ? null : reader.GetString("phone");

                    Customer customer = new Customer(id, user.ID, name, observation, nit, address, phone, balance);
                    customers.Add(customer);
                }
            }

            foreach(Customer customer in customers)
            {
                UpdateCustmerState(customer);
            }

            return customers;
        }

        public static bool AddCustomer(int userId, string customerName, string observation, string nit, string address, 
            string phone)
        {
            customerName = PonerComillas(customerName);
            observation = PonerNullOrComillas(observation);
            nit = PonerNullOrComillas(nit);
            address = PonerNullOrComillas(address);
            phone = PonerNullOrComillas(phone);

            string query = "INSERT INTO customer(user_id, customer_name, observation, nit, address, phone) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4}, {5})", userId, customerName,
                observation, nit, address, phone);

            if(ModificarTabla(query))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Este metodo actualiza el saldo del cliente y crea una deuda de forma segura
        /// </summary>
        /// <param name="userId">El id del usuario al que está asociado el cliente</param>
        /// <param name="customerID">El id del cliente al que se le va a abonar una deuda</param>
        /// <param name="date">La fecha del compromiso</param>
        /// <param name="description">La descripcion o comcepto de la deuda</param>
        /// <param name="amount">El saldo a favor del acreedor y que se le impone al cliente</param>
        /// <returns></returns>
        public static bool AddDebt(int userId, int customerID, DateTime date, string description, decimal amount)
        {
            string currentDate = PonerComillas(DateTime.Now.ToString("yyyy-MM-dd"));

            description = PonerComillas(description);
            string dateString = PonerComillas(date.ToString("yyyy-MM-dd"));
            string query = "START TRANSACTION; ";
            query += "UPDATE customer ";
            query += string.Format("SET balance = balance + {0} ", amount);
            query += string.Format("WHERE (user_id = {0} AND customer_id = {1}); ", userId, customerID);
            query += "INSERT INTO debt (customer_id, user_id, date_of_debt, description, amount) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4});", customerID, userId, dateString, description, amount);
            query += string.Format("INSERT INTO customer_update(customer_id, user_id, date_of_update) VALUES ");
            query += string.Format("({0}, {1}, {2}); COMMIT;", customerID, userId, currentDate);

            if(ModificarTabla(query))
            {
                return true;
            }

            return false;
        }

        public static bool AddPayment(int userId, int customerID, DateTime date, string observation, decimal amount, bool efectivo, bool tarjeta)
        {
            string currentDate = PonerComillas(DateTime.Now.ToString("yyyy-MM-dd"));

            observation = PonerComillas(observation);
            string dateString = PonerComillas(date.ToString("yyyy-MM-dd"));
            string query = "START TRANSACTION; ";
            query += "UPDATE customer ";
            query += string.Format("SET balance = balance - {0} ", amount);
            query += string.Format("WHERE (user_id = {0} AND customer_id = {1});", userId, customerID);
            query += "INSERT INTO payment (customer_id, user_id, date_of_payment, observation, amount, efectivo, tarjeta) VALUES ";
            query += string.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}); ", 
                customerID, userId, dateString, observation, amount, efectivo, tarjeta);
            query += string.Format("INSERT INTO customer_update(customer_id, user_id, date_of_update) VALUES ");
            query += string.Format("({0}, {1}, {2}); COMMIT;", customerID, userId, currentDate);

            if (HacerConsulta(query))
            {
                return true;
            }

            return false;
        }

        public static bool UpdateCustomerDebt(int debt_id, int userId, int customerId, string description, DateTime transactionDate)
        {
            string date = PonerComillas(transactionDate.ToString("yyyy-MM-dd"));
            description = PonerComillas(description);

            string query = "UPDATE debt SET ";
            query += string.Format("description = {0}, date_of_debt = {1} ", description, date);
            query += string.Format("WHERE debt_id = {0} AND customer_id = {1} AND user_id = {2}", debt_id, customerId, userId);

            return ModificarTabla(query);
        }

        public static bool UpdateCustomerPayment(int paymentId, int userId, int customerId, string description, DateTime transactionDate)
        {
            string date = PonerComillas(transactionDate.ToString("yyyy-MM-dd"));
            description = PonerComillas(description);

            string query = "UPDATE payment SET ";
            query += string.Format("observation = {0}, date_of_payment = {1} ", description, date);
            query += string.Format("WHERE payment_id = {0} AND customer_id = {1} AND user_id = {2}", paymentId, customerId, userId);

            return ModificarTabla(query);
        }

        public static List<string> ConsultCustomersUpdate(DateTime date, int userID)
        {
            string currentDate = PonerComillas(date.ToString("yyyy-MM-dd"));
            List<string> result = new List<string>();

            string query = "SELECT DISTINCT t1.customer_name, t1.balance FROM customer as t1 ";
            query += "JOIN customer_update as t2 ON t2.customer_id = t1.customer_id ";
            query += string.Format("AND t2.date_of_update= {0} and t2.user_id = {1}", currentDate, userID);

            if (HacerConsulta(query))
            {
                while(reader.Read())
                {
                    string customer = reader.GetString(0);
                    customer += "(" + reader.GetDecimal(1).ToString("c") + ")";
                    result.Add(customer);
                }
            }

            return result;
        }

        /// <summary>
        /// Este metodo consulta a la base de datos el saldo del cliente pasado como parametro
        /// </summary>
        /// <param name="customer"></param>
        public static void UpdateCustmerState(Customer customer)
        {
            PointsSystem.EstablecerParametros(0.1d, 0.04d, 0.02d, -0.02d, -0.04d, -0.08d, -0.1d, PeriodoDeCobro.Mensual);

            List<CustomerTransaction> transactions = ReadCustomerTransaction(customer);         //Las transacciones del clientes
            List<PaymentTracking> paymentsTracking = new List<PaymentTracking>();               //Las estadisticas de pago del cliente
            DateTime? cutOffDate = null;                //Fecha de corte
            DateTime? paymentDate = null;               //Fecha del ultimo pago
            DateTime? lastDateDebt = null;              //Fecha del ultimo compromiso adquirido
            decimal? paymentAmount = null;              //Monto o saldo del ultimo pago realizado
            decimal? debtAmount = null;                 //Es el monto del ultimo compromiso adquirido
            double? paymentPercentage = null;           //Porcentaje de la deuda pagado
            int points = 0;                             //Los puntos acumulados
            decimal balance = 0m;
            decimal? averagePayment = null;             //Es el promedio de pago del cliente
            decimal acumulatePayment = 0m;           //Es el acumulado de los pagos del cliente

            for (int index = 0; index < transactions.Count; index++)
            {
                CustomerTransaction t = transactions[index];
                
                if (index == 0)
                {
                    lastDateDebt = t.Fecha;
                    cutOffDate = t.Fecha.AddDays(DateTime.DaysInMonth(t.Fecha.Year, t.Fecha.Month));
                    debtAmount = t.Deuda;
                }
                else
                {
                    if (t.Abono > 0)
                    {
                        paymentDate = t.Fecha;
                        paymentAmount = t.Abono;
                        PaymentTracking pT = new PaymentTracking(cutOffDate.Value, paymentDate.Value, balance, paymentAmount.Value);
                        paymentPercentage = pT.PaymentPercentage;
                        
                        //Si el saldo del cliente es cero entonces la fecha de corte se vuelve null
                        if(t.Saldo==0)
                        {
                            cutOffDate = null;
                        }
                        else
                        {
                            cutOffDate = paymentDate.Value.AddDays(30);
                        }
                        
                        points += pT.Points;
                        acumulatePayment += t.Abono;

                        /*
                        if (points < 0)
                        {
                            points = 0;
                        }
                        */
                        paymentsTracking.Add(pT);
                        averagePayment = acumulatePayment / paymentsTracking.Count;
                    }
                    else
                    {
                        lastDateDebt = t.Fecha;
                        debtAmount = t.Deuda;

                        if (!cutOffDate.HasValue)
                        {
                            cutOffDate = lastDateDebt.Value.AddDays(30);
                        }
                    }
                }

                balance = t.Saldo;
            }

            //Finalmente se actualiza el cliente
            customer.CutoffDate = cutOffDate;
            customer.LastPaymentDate = paymentDate;
            customer.LastPayment = paymentAmount;
            customer.PaymentPercentage = paymentPercentage;
            customer.Transactions = transactions;
            customer.PaymentsTracking = paymentsTracking;
            if(customer.CustomerName.Contains("Mild"))
            {
                double a = 1;
            }
            customer.CustomerTracking = PointsSystem.DefineScore2(transactions);
            customer.Balance = balance;
            customer.Points = PointsSystem.LastPoints;
            customer.LastDebtDate = lastDateDebt;
            customer.LastDebtAmount = debtAmount;
            customer.AveragePayment = averagePayment;
            

            
        }
        
        /// <summary>
        /// Este metodo recupera los cobros y los pagos de la base de datos y los
        /// convierte en transacciones dobles para determinar los saldos
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static List<CustomerTransaction> ReadCustomerTransaction(Customer customer)
        {
            int id;
            DateTime fecha;
            string description;
            decimal deuda=0m;
            decimal abono=0m;

            List<CustomerTransaction> transactions = new List<CustomerTransaction>();
            string query1 = "SELECT debt_id, date_of_debt, description, amount FROM debt WHERE customer_id = " + customer.CustomerID;
            string query2 = "SELECT payment_id, date_of_payment, observation, amount FROM payment WHERE customer_id = " + customer.CustomerID;

            if(HacerConsulta(query1))
            {
                while(reader.Read())
                {
                    id = reader.GetInt32("debt_id");
                    fecha = reader.GetDateTime("date_of_debt");
                    description = reader.GetString("description");
                    deuda = reader.GetDecimal("amount");
                    CustomerTransaction cT = new CustomerTransaction(id, fecha, description, deuda, abono);
                    transactions.Add(cT);
                }

                deuda = 0m; 

                if(HacerConsulta(query2))
                {
                    while (reader.Read())
                    {
                        id = reader.GetInt32("payment_id");
                        fecha = reader.GetDateTime("date_of_payment");
                        description = reader.GetString("observation");
                        abono = reader.GetDecimal("amount");
                        CustomerTransaction cT = new CustomerTransaction(id, fecha, description, deuda, abono);
                        transactions.Add(cT);
                    }

                    transactions.Sort((x, y) => x.Fecha.CompareTo(y.Fecha));
                    decimal saldo = 0m;
                    foreach(CustomerTransaction t in transactions)
                    {
                        saldo += t.Deuda - t.Abono;
                        t.Saldo = saldo;
                    }
                }
                else
                {
                    transactions.Clear();
                }
            }

            return transactions;
        }

        /// <summary>
        /// Este metodo actualiza los datos del usuario en la base de datos
        /// </summary>
        /// <returns></returns>
        public static bool UpdateCustomer(int userId, int customerId, string name, string observation, string phone,
            string address, string nit)
        {
            name = PonerComillas(name);
            observation = PonerNullOrComillas(observation);
            phone = PonerNullOrComillas(phone);
            address = PonerNullOrComillas(address);
            nit = PonerNullOrComillas(nit);

            string query = "UPDATE customer SET ";
            query += string.Format("customer_name = {0}, observation = {1}, nit = {2}, ", name, observation, nit);
            query += string.Format("address = {0}, phone = {1} ", address, phone);
            query += string.Format("WHERE (user_id = {0} AND customer_id = {1})", userId, customerId);

            return ModificarTabla(query); 
        }

        /// <summary>
        /// Este metodo elimina de forma segura un cliente de la base de datos
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int customerID)
        {
            string query = "DELETE FROM customer WHERE customer_id = " + customerID;

            return HacerConsulta(query);
        }

        #endregion

        /// <summary>
        /// Este metodo crea una nueva categoría en la base de datos
        /// </summary>
        /// <param name="name">Nombre de la categoría</param>
        /// <param name="categoryClass">El rango de la categoría, entre mas alto mas especifica es</param>
        /// <returns></returns>
        public static bool CreateCategory(string name, int categoryClass, int userId)
        {
            name = PonerComillas(name);
            string query = "START TRANSACTION; ";
            query += "INSERT INTO category(category_name, class) VALUES ";
            query += string.Format("({0}, {1}); ", name, categoryClass);
            query += "SET @last_category_id = LAST_INSERT_ID(); ";
            query += "INSERT INTO user_has_category(user_id, category_id) VALUES ";
            query += string.Format("({0}, @last_category_id); ", userId);
            query += "COMMIT";

            return ModificarTabla(query);
        }

        /// <summary>
        /// Este metodo entrega un listado de categorias recuperadas de la base de datos
        /// </summary>
        /// <returns></returns>
        public static List<Category> ReadAllCategories(int userId)
        {
            List<Category> categories = new List<Category>();
            string query = "SELECT * FROM category AS t1 JOIN user_has_category AS t2 ON ";
            query += string.Format("(t2.user_id = {0} AND t1.category_id = t2.category_id) ", userId);
            query += "ORDER BY t1.class ASC";

            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("category_id");
                    string name = reader.GetString("category_name");
                    int categoryClass = reader.GetInt32("class");

                    Category c = new Category(id, name, categoryClass);
                    categories.Add(c);
                }//Fin de while
            }//Fin de if

            foreach(Category c in categories)
            {
                ReadSubcategories(c);
            }

            return categories;
        }//Fin del metodo

        /// <summary>
        /// Este metodo busca en la base de datos los identificadores de las subcategorias asociadas
        /// a la cateoria pasada como parametro
        /// </summary>
        /// <param name="c"></param>
        private static void ReadSubcategories(Category c)
        {
            List<int> ids = new List<int>();
            string query = "SELECT son_id AS id FROM category_has_subcategory WHERE father_id = " + c.ID;

            if(HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("id");
                    ids.Add(id);
                }
            }

            c.AddSubcategories(ids);
        }

        public static bool MatchCategories(int fatherId, int sonId)
        {
            string query = "INSERT INTO category_has_subcategory(father_id, son_id) VALUES ";
            query += string.Format("({0}, {1})", fatherId, sonId);

            return ModificarTabla(query);
        }

        public static bool RemoveRlationship(int fatherId, int sonId)
        {
            string query = "DELETE FROM category_has_subcategory WHERE ";
            query += "father_id = " + fatherId;
            query += " AND son_id = " + sonId;
            return ModificarTabla(query);
        }


        public static void MetaDataConsult(int userId, out DateTime? minDate, out DateTime? maxDate, out decimal balance, 
            out decimal deposit, out decimal egress)
        {
            string query1 = string.Format("(SELECT MIN(transacction_date) FROM transacction WHERE user_id = {0} AND transfer=0) UNION ALL ", userId);
            query1 += string.Format("(SELECT MAX(transacction_date) FROM transacction WHERE user_id = {0} AND transfer=0)", userId);
            string query2 = string.Format("(SELECT SUM(amount) FROM transacction WHERE user_id = {0} AND transfer=0) UNION ALL ", userId);
            query2 += string.Format("(SELECT SUM(amount) FROM transacction WHERE user_id = {0} AND transfer=0 AND amount>0) UNION ALL ", userId);
            query2 += string.Format("(SELECT ABS(SUM(amount)) FROM transacction WHERE user_id = {0} AND transfer=0 AND amount<0)", userId);

            if(HacerConsulta(query1))
            {
                //Se lee la fecha de la primera transaccion
                reader.Read();
                minDate = reader.IsDBNull(0) ? (DateTime?) null : reader.GetDateTime(0);

                //Si la fecha no es null, entonces se puede leer los siguientes datos
                if (minDate != null)
                {
                    reader.Read();
                    maxDate = reader.IsDBNull(0) ? (DateTime?)null : reader.GetDateTime(0);

                    if(HacerConsulta(query2))
                    {
                        reader.Read();
                        balance = reader.IsDBNull(0) ? 0m : reader.GetDecimal(0);

                        reader.Read();
                        deposit = reader.IsDBNull(0) ? 0m : reader.GetDecimal(0);

                        reader.Read();
                        egress = reader.IsDBNull(0) ? 0m : reader.GetDecimal(0);
                    }
                    else
                    {
                        balance = 0m;
                        deposit = 0m;
                        egress = 0m;
                    }
                }
                else
                {
                    maxDate = null;
                    balance = 0m;
                    deposit = 0m;
                    egress = 0m;
                }
            }
            else
            {
                minDate = null;
                maxDate = null;
                deposit = 0m;
                egress = 0m;
                balance = 0m;
            }
        }

        /// <summary>
        /// Este metodo consulta las transacciones segun la ruta de categorias pasada como parametros
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static List<CashboxTransaction> RecoverTransaction(List<Category> categories, DateTime since, DateTime until)
        {
            List<CashboxTransaction> result = new List<CashboxTransaction>();

            string query = "";
            if (categories.Count == 1)
            {
                query = WriteQueryTableClass1(categories[0].ID, since, until);
            }
            else if(categories.Count == 2)
            {
                string table1 = WriteQueryTableClass1(categories[0].ID, since, until);
                string table2 = WriteQueryTableClass1(categories[1].ID, since, until);

                query = WriteQueryTableClass2(table1, table2);
            }
            else
            {
                string table1 = WriteQueryTableClass1(categories[0].ID, since, until);
                string table2 = WriteQueryTableClass1(categories[1].ID, since, until);
                query = WriteQueryTableClass2(table1, table2);

                for (int index = 2; index < categories.Count; index++)
                {
                    table2 = WriteQueryTableClass1(categories[index].ID, since, until);
                    query = WriteQueryTableClass2(query, table2);
                }
            }

            if (HacerConsulta(query))
            {
                while(reader.Read())
                {
                    int id = reader.GetInt32("transacction_id");
                    DateTime date = reader.GetDateTime("transacction_date");
                    string description = reader.GetString("description");
                    decimal amount = reader.GetDecimal("amount");
                    CashboxTransaction t = new CashboxTransaction(id, date, description, amount, false); //Si se busca por categoría no encontrará ninguna con transferencia igual a true
                    result.Add(t);
                }
            }

            return result;
        }

        /// <summary>
        /// Este metodo escribe las instrucciones para consultar los datos de las transacciones que pertenecen a una 
        /// categoría. 
        /// </summary>
        /// <param name="categoryId">Categoría a consultar</param>
        /// <returns></returns>
        private static string WriteQueryTableClass1(int categoryId, DateTime since, DateTime until)
        {
            string sinceText = PonerComillas(since.ToString("yyyy-MM-dd"));
            string untilText = PonerComillas(until.ToString("yyyy-MM-dd"));

            string query = "SELECT t1.transacction_id, t1.transacction_date, t1.description, t1.amount ";
            query += "FROM transacction as t1 JOIN transacction_has_category as t2 ";
            query += "ON (t2.transacction_id = t1.transacction_id AND ";
            query += string.Format("t2.category_id = {0} AND t1.transacction_date >= {1} AND t1.transacction_date <= {2})", categoryId, sinceText, untilText);


            //Futura actualizacion: Solo debo modificar este codigo para seguir filtrando entre fechas

            return query;
        }

        /// <summary>
        /// Este metodo retorna las instrucciones para la base de datos y que sirve para recuperar las transacciones
        /// que se encuentran en la tabla 1 y que coinciden con las transacciones de la tabla 2
        /// </summary>
        /// <param name="table1">Tabla principal, con numero mayor o igual de transacciones de la tabla 2</param>
        /// <param name="table2">tabla con la cual se va a comprar la tabla 1 para seguir filtrando</param>
        /// <returns></returns>
        private static string WriteQueryTableClass2(string table1, string table2)
        {
            string query = "SELECT t3.transacction_id, t3.transacction_date, t3.description, t3.amount ";
            query += string.Format("FROM ({0}) AS t3 ", table1);
            query += string.Format("JOIN ({0}) AS t4 ", table2);
            query += "ON t3.transacction_id = t4.transacction_id";
            return query;
        }

        /// <summary>
        /// Este metodo agrega las comilla a los textos 
        /// que van a ser ingresados a la base de datos
        /// </summary>
        /// <param name="texto">Texto a encomillar</param>
        /// <returns>Cadena de texto con el formato adecuado</returns>
        private static string PonerComillas(string texto)
        {
            string resultado = "\"";
            resultado += texto;
            return resultado += "\"";
        }

        /// <summary>
        /// Si el texto se encuentra en blanco o null cambia el valor por null y que este pueda se ingresado en 
        /// la base de datos
        /// </summary>
        /// <param name="texto"></param>
        /// <returns></returns>
        private static string PonerNullOrComillas(string texto)
        {
            return (string.IsNullOrWhiteSpace(texto) || string.IsNullOrEmpty(texto)) ? "NULL" : PonerComillas(texto);
        }
    }
}
