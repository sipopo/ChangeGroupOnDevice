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
                                                                
                string grName = Properties.Settings.Default.grName;
                string old_group = Properties.Settings.Default.old_group;
                string new_group = Properties.Settings.Default.new_group;

                Log("Working with group " + grName);
                Log("We change group " + old_group + " to " + new_group + " in city " + grName);
                Console.WriteLine("We change group {0} to {1} in city {2}", old_group, new_group, grName);
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
                        //ChangeDeviceCluster(account.ExternalID, PrinMan);

                        if (i > 2) break; // temporary for testing
                    }
                
                }

                //Check Corbinamsk00_5496044_1
                //ChangeDeviceCluster("Corbinamsk00_5496044_1", PrinMan);

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
                string old_group = Properties.Settings.Default.old_group;
                string new_group = Properties.Settings.Default.new_group;

                Account[] Accounts = PrinMan.ReadAccount(account);

                PrincipalManagementInterfaceSoap PrinInterface = new PrincipalManagementInterfaceSoap();
                PrinInterface.Credentials = new NetworkCredential("IPTVServices", "P23R@vor", "BRMSK");
                PrinInterface.Url = "http://78.107.199.132/bss/PrincipalManagement.asmx";

                foreach (Account ac in Accounts)
                {
                    Device[] devices = ac.Devices;
                    foreach (Device dev in devices)
                    {
                        Log("Working with device " + dev.ExternalID );
                        Console.WriteLine("Working with device " + dev.ExternalID);

                        DeviceGuidPrincipalId devGUID = new DeviceGuidPrincipalId();
                        devGUID.Id = dev.ID;

                        GroupMembership[] Groups = PrinInterface.GetGroupMemberships(devGUID);
                        foreach (GroupMembership group in Groups)
                        {
                            GroupPrincipalExternalId groupExternalID = group.GroupExternalId;
                            if  (groupExternalID.Id.ToString().Contains(old_group))
                            {
                                Console.WriteLine("We found old Group  " + old_group);

                                Console.WriteLine("Try to change group " + old_group + " on " + new_group + " for " + dev.ExternalID);
                                Log("Try to change group " + groupExternalID.Id + " on " + new_group + " for " + dev.ExternalID);
                                // Here we must change cluster_group

                                GroupPrincipalExternalId[] GroupsToRemove = new GroupPrincipalExternalId[1];
                                GroupsToRemove[0] = groupExternalID;

                                GroupPrincipalExternalId[] GroupsToAdd = new GroupPrincipalExternalId[1];
                                GroupsToAdd[0] = new GroupPrincipalExternalId();
                                GroupsToAdd[0].Id = new_group; // new_group;

                                PrinInterface.RemoveGroupMemberships(devGUID, GroupsToRemove);
                                PrinInterface.AddGroupMemberships(devGUID, GroupsToAdd);                                                                                                                                
                            }                            
                        }

                        //
                        
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
