﻿using System;
using System.Linq;
using System.ServiceModel;
using OMSSCADACommon;
using TransactionManagerContract;
using SCADA.RealtimeDatabase;
using SCADA.ConfigurationParser;

namespace SCADA.ClientHandler
{
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SCADATransactionService : ITransactionSCADA
    {
        private static string modifiedConfigFile = "NewScadaModel.xml";
        private static string currentConfigFile = "ScadaModel.xml";

        private DBContext dbContext = null;

        public SCADATransactionService()
        {
            dbContext = new DBContext();
        }

        /// <summary>
        /// Check if there is ANY free space in controller; at this point we do not know if delta will contain 1 or 10 measurements (analog or/and digital)
        /// so we only check if is it possible to add minimal memory occupying element
        /// </summary>
        public void Enlist()
        {
            Console.WriteLine("\nTransaction started -> \nPozvan je Enlist na SCADA");

            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();
        
            bool isSuccessfull = false;

            // at this point, we will only check if there is a free space for any DIGITALS or any ANALOGS
            var availableRtus = dbContext.GettAllRTUs().Values.Where(r => r.FreeSpaceForDigitals == true ||
                                                                    r.FreeSpaceForAnalogs == true).ToList();
            if (availableRtus.Count != 0)
                isSuccessfull = true;

            try
            {
                callback.CallbackEnlist(isSuccessfull);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //callback.CallbackEnlist(false);
            }
        }

        /// <summary>
        /// Trying to apply, APPLYING if it is possible, and write new configuration to new file
        /// </summary>
        /// <param name="delta"></param>
        public void Prepare(ScadaDelta delta)
        {
            Console.WriteLine("Pozvan je Prepare na SCADA");

            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();

            if (dbContext.ApplyDelta(delta))
            {
                try
                {
                    ScadaModelParser parser = new ScadaModelParser();

                    // to do:
                    // mozda ove serijaliyacije da budu taskovi_
                    // novu konfiguraciju cuvamo u fajlu
                    parser.SerializeScadaModel(modifiedConfigFile);

                    Console.WriteLine("Prepare true");
                    callback.CallbackPrepare(true);
                }
                catch (Exception ex)
                {
                    ScadaModelParser parser = new ScadaModelParser();
                    parser.DeserializeScadaModel(); // returning to old state (state was previosuly changed in apply delta)

                    Console.WriteLine(ex.Message);
                    Console.WriteLine("1Prepare false");
                    callback.CallbackPrepare(false);
                }
            }
            else
            {
                ScadaModelParser parser = new ScadaModelParser();
                parser.DeserializeScadaModel(); // returning to old state (state was previosuly changed in apply delta)
                Console.WriteLine("2Prepare false");
                callback.CallbackPrepare(false);
            }
        }

        /// <summary>
        /// Setting Configuration to new file.
        /// </summary>
        public void Commit()
        {
            Console.WriteLine("Pozvan je Commit na SCADA");
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();

            ScadaModelParser parser = new ScadaModelParser();

            // to do check this
            //parser.SwapConfigs(currentConfigFile, modifiedConfigFile);

            callback.CallbackCommit("Commited on SCADA");          
        }

        /// <summary>
        /// Returning to old config file, initialize database again (deserializing from file)
        /// </summary>
        public void Rollback()
        {
            Console.WriteLine("Pozvan je Rollback na SCADA");
            IDistributedTransactionCallback callback = OperationContext.Current.GetCallbackChannel<IDistributedTransactionCallback>();

            ScadaModelParser parser = new ScadaModelParser();
            //parser.SwapConfigs(newConfigFile, currentConfigFile);
            parser.DeserializeScadaModel(); // returning to old state (changed in apply delta)

            callback.CallbackRollback("Something went wrong on SCADA");
        }
    }
}
