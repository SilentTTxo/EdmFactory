using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Drawing;

namespace EDM
{
    class Program
    {
        class EdmModel
        {
            public string Base { get; set; }
            public string row1 { get; set; }
            public string row2 { get; set; }
            public string row3 { get; set; }
            public string row4 { get; set; }
        }
        static void Main(string[] args)
        {
            //处理图片
            /*Console.WriteLine("输入每一行图片个数，逗号隔开：");
            string input = Console.ReadLine();
            string[] temp = input.Split(' ');
            int[] sum = new int[temp.Length];
            for(int i=0;i<temp.Length;i++)
            {
                sum[i] = Convert.ToInt32(temp[i]);
            }*/
            Console.WriteLine("Tomato的EDM生成器");
            List<int> sumL = new List<int>();
            List<int> sumML = new List<int>();
            int[] sum = null;
            var path = @"./img";//文件夹路径
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fiList = null;
            if (dir.Exists)
            {
                 fiList = dir.GetFiles();
                int y = 0,x = 0;
                foreach (FileInfo fi in fiList)
                {
                    fi.MoveTo("./img/" + y++ + Path.GetExtension(fi.FullName));
                }
                for (y=0;y < fiList.Length;y++)
                {
                    

                    //自动识别图片宽度
                    int delta = 20;
                    Image img = Image.FromFile(fiList[y].FullName);
                    int width = img.Width;
                    int a = 0;
                    if (width <= 150 + delta) a = 4;
                    else if (width <= 200 + delta) a = 3;
                    else if (width <= 300 + delta) a = 2;
                    else if (width <= 600 + delta) a = 1;
                    sumL.Add(a);
                    int b = 0;
                    for(int i=0;i< a; i++)
                    {
                        Image imgt = Image.FromFile(fiList[y+i].FullName);
                        b += imgt.Width;
                        imgt.Dispose();
                    }
                    y += a - 1;
                    b = (600 - b) / (a == 1 ? 1 : a-1);
                    if (b < 0) b = 0;
                    if(a!=1) b -= 4;
                    sumML.Add(b);
                    img.Dispose();
                }
                sum = sumL.ToArray();
                
                //计算部分宽度
                y = 0;x = 0;
                foreach(FileInfo fi in fiList)
                {
                    
                   if (y == sum[x]) { x++; y = 0; }
                   try
                   {
                       fi.MoveTo("./img/" + (x + 1) + "-" + (y + 1) + Path.GetExtension(fi.FullName));
                       Console.WriteLine("处理图片:" + fi.Name);
                   }
                   catch
                   {
                       Console.WriteLine("文件已存在");
                   }

                   y++;
                }
            }
            int[] summl = sumML.ToArray();

            //生成模板
            string json = System.IO.File.ReadAllText(@"data.json");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            EdmModel edmModel = serializer.Deserialize<EdmModel>(json);
            string content = "";
            int xx = 0;
            int yy = 0;
            for (int i = 0; i < sum.Length; i++)
            {
                switch (sum[i])
                {
                    case 1: content += string.Format(edmModel.row1, fiList[xx++].Name); yy++;  break;
                    case 2: content += string.Format(edmModel.row2, fiList[xx++].Name, fiList[xx++].Name, summl[yy++]); break;
                    case 3: content += string.Format(edmModel.row3, fiList[xx++].Name, fiList[xx++].Name, fiList[xx++].Name, summl[yy++]); break;
                    case 4: content += string.Format(edmModel.row4, fiList[xx++].Name, fiList[xx++].Name, fiList[xx++].Name, fiList[xx++].Name, summl[yy++]); break;
                }
            }
            string re = edmModel.Base.Replace("{{Content}}", content);
            System.IO.File.WriteAllText(@"demo.html", re, Encoding.UTF8);
            Console.WriteLine("邮件已生成...");
            Console.ReadLine();
        }
    }
}
