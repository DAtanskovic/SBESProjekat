﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using System.ServiceModel;
using CertManager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using Data;

namespace Client
{
    public class WCFClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
    {
        IWCFContract factory;



        public WCFClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = Manager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = CreateChannel();
        }

        public void TestCommunication()
        {
            try
            {
                factory.TestCommunication();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }

        public bool CreateDataBase(string a)
        {
            try
            {
               
                return factory.CreateDataBase(a);
            }
            catch (Exception e)
            {
                Console.WriteLine("[CreateDataBase] ERROR = {0}", e.Message);
                return false;
            }
        }

        public bool Add(EnergyConsumptionModel item)
        {
            try
            {
                return factory.Add(item);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Add] ERROR = {0}", e.Message);
                return false;
            }
        }
        public List<EnergyConsumptionModel> Read()
        {

            try
            {
                return factory.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Read] ERROR = {0}", e.Message);
                return null;
            }


        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }

        
    }
}
