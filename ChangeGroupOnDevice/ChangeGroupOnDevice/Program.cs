using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ChangeGroupOnDevice
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Make request");
            try {
                Log("Start programm");
                PrincipalManagementSoap  PrinMan = new PrincipalManagementSoap();
                PrinMan.Credentials = new NetworkCredential("IPTVServices", "P23R@vor", "BRMSK");
                PrinMan.Url = "http://78.107.199.132/bss/PrincipalManagement.asmx";


                string grName = "City_8";

                Log("Working with group " + grName);
                Group[] Groups = PrinMan.ReadGroup(grName);

                foreach (Group group in Groups)
                {
                    Console.WriteLine("In group " + group.ExternalID.ToString() + " the number of account is " + group.Principals.Length.ToString() );
                    Log("In group " + group.ExternalID.ToString() + " the number of account is " + group.Principals.Length.ToString());

                    Principal[] Accounts = group.Principals;

                    // breaking if
                    int i = 0; // temporary for testing

                    foreach (Principal account in Accounts)
                    {
                        i = i + 1; // temporary for testing
                        Console.WriteLine("Working with : " + account.ExternalID);
                        Log("Working with : " + account.ExternalID);
                        ChangeDeviceCluster(account.ExternalID, PrinMan);

                        if (i > 2) break; // temporary for testing
                    }
                
                }
             
            
                Console.WriteLine("The count of groups " + Groups.Length.ToString());
            } // try 
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                Log("Error : " + e.Message);
            }

            finally
            { 
                Console.WriteLine("Done!");
                Log("Done !");
                Console.ReadLine();
            }
        }

        public static void ChangeDeviceCluster(string account, PrincipalManagementSoap PrinMan)
        {
            try { 
                Account[] Accounts = PrinMan.ReadAccount(account);
            
                foreach (Account ac in Accounts)
                {
                    Device[] devices = ac.Devices;
                    foreach (Device dev in devices)
                    {
                        Log("Working with device " + dev.ExternalID );
                        Console.WriteLine("Working with device " + dev.ExternalID);
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
                Log("Error : " + e.Message);
            }

        } // end ChangeDeviceCluster
        public static void Log(string logMessage)
        {
            //string path = Properties.Settings.Default.LogPath;
            string path = ".\\";


            string filename = Process.GetCurrentProcess().ProcessName + "_" + DateTime.Today.ToString("yyyy-MM-dd") + ".log";

            try
            {
                using (StreamWriter w = File.AppendText(path + "\\" + filename))
                {
                    w.Write("{0}\t", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    w.WriteLine(logMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error to write file log " + e.ToString());
            }

        }// end of log
    }
}
