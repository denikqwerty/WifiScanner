using System.Collections.Generic;
using System.Linq;

namespace WifiScanner.NetFolders
{
    class TestShares
    {
        public static List<Share> SharedFolders(string host)
        {
            List<Share> lstShares = new List<Share>();
            string server = host;

            if (server != null && server.Trim().Length > 0)
            {
                ShareCollection shi = ShareCollection.GetShares(server);
                if (shi != null)
                {
                    foreach (Share si in shi)
                    {
                        if ((si.ShareType == ShareType.Disk && !si.NetName.Contains('$')) || si.ShareType == ShareType.Printer)
                        {
                            lstShares.Add(si);
                        }
                        else
                        {

                        }
                        /*Console.WriteLine("{0}: {1} [{2}]",
                            si.ShareType, si, si.Path);

                        // If this is a file-system share, try to
                        // list the first five subfolders.
                        // NB: If the share is on a removable device,
                        // you could get "Not ready" or "Access denied"
                        // exceptions.
                        // If you don't have permissions to the share,
                        // you will get security exceptions.
                        if (si.IsFileSystem)
                        {
                            try
                            {
                                System.IO.DirectoryInfo d = si.Root;
                                System.IO.DirectoryInfo[] Flds = d.GetDirectories();
                                for (int i = 0; i < Flds.Length && i < 5; i++)
                                    Console.WriteLine("\t{0} - {1}", i, Flds[i].FullName);

                                Console.WriteLine();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\tError listing {0}:\n\t{1}\n",
                                    si.ToString(), ex.Message);
                            }
                        }*/
                    }
                    return lstShares;
                }
                else
                    return lstShares;
            }
            return lstShares;
        }
    }
}
