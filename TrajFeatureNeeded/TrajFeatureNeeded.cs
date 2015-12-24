using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;


namespace DataProc.TrajFeatureNeeded
{
    class TrajFeatureNeeded
    {
        public static void FeatureNeeded(string filePath, string destPath)
        {
            string newFilePath = Path.Combine(destPath, "fea-"+Path.GetFileNameWithoutExtension(filePath) + ".csv");
            StreamWriter sw = new StreamWriter(newFilePath);
            StreamReader sr = new StreamReader(filePath);
            string iline = null;
            while ((iline = sr.ReadLine()) != null)
            {
                if (iline[0] != '#')
                {
                    continue;
                }
                sw.WriteLine(iline.Substring(0, iline.Length - 3));
            }
            sw.Flush();
            sw.Dispose();
            sr.Close();
        }
        static void Main(string[] args)
        {
            int NUMTHREADS = 4;
            if (args.Length != 2)
            {
                if (args.Length == 3)
                {
                    NUMTHREADS = int.Parse(args[2]);
                }
                else
                {
                    return;
                }
            }
            if (!Directory.Exists(args[1]))
            {
                Directory.CreateDirectory(args[1]);
            }




            List<string>[] fileSets = new List<string>[NUMTHREADS];
            for (int i = 0; i < fileSets.Length; i++)
            {
                fileSets[i] = new List<string>();
            }
            var fileList = Directory.EnumerateFiles(args[0], "*.csv", SearchOption.TopDirectoryOnly);
            int cnt = 0;
            foreach (var item in fileList)
            {
                fileSets[cnt % NUMTHREADS].Add(item);
                cnt++;
            }
            Thread[] thr = new Thread[NUMTHREADS];
            for (int i = 0; i < thr.Length; i++)
            {
                int tmpi = i;
                thr[i] = new Thread(delegate ()
                {
                    foreach (var item in fileSets[tmpi])
                    {
                        FeatureNeeded(item, args[1]);
                    }
                });
            }
            for (int i = 0; i < thr.Length; i++)
            {
                thr[i].Start();
            }
            for (int i = 0; i < thr.Length; i++)
            {
                thr[i].Join();
            }
        }
    }
}
