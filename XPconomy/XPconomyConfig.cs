using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using System.Xml.Serialization;

namespace XPconomy
{
    public class XPconomyConfig : IRocketPluginConfiguration
    {
        public string CurrencyName;
        public string CurrencySymbol;
        public uint StartingBalance;
        public bool SalaryEnabled;
        public uint SalaryInterval;

        public int DatabasePort;
        public string DatabaseName;
        public string DatabaseAdress;
        public string DatabaseUsername;
        public string DatabasePassword;

        public bool Debug;

        [XmlArrayItem(ElementName = "Salary")]
        public List<Salary> Salaries;

        public void LoadDefaults()
        {
            CurrencyName = "Dollars";
            CurrencySymbol = "$";
            StartingBalance = 200;
            SalaryEnabled = false;
            SalaryInterval = 3600;

            DatabasePort = 3306;
            DatabaseName = "Unturned";
            DatabaseAdress = "localhost";
            DatabaseUsername = "Username";
            DatabasePassword = "Password";
            Debug = false;

            Salaries = new List<Salary>
            {
                new Salary {SalaryPermission = "Salary.Police", SalaryAmount = 400},
                new Salary {SalaryPermission = "Salary.EMS", SalaryAmount = 500},
                new Salary {SalaryPermission = "Salary.Civilian", SalaryAmount = 200}
            };
        }


    }

    public class Salary
    {
        public Salary() { }
        public string SalaryPermission;
        public uint SalaryAmount;
    }
}
