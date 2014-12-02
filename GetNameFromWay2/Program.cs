using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNameFromWay2
{
    class Program
    {
        static void Main(string[] args)
        {
            GetPositionName getNames = new GetPositionName();

            bool stop = false;
            while (!stop)
            {
                Console.Clear();
                Console.WriteLine("Para encerrar digite X");
                Console.WriteLine("Digite um nome para pesquisar");
                string lname = Console.ReadLine();

                if (lname.Trim() == "X")
                {
                    stop = true;
                    continue;
                }
                    
                if (lname.Trim() == string.Empty || lname.Length < 3)
                { 
                    Console.WriteLine("Nome inválido");
                    continue;
                }

                Int64[] lPos = getNames.searchName(lname);
                if (lPos[0] == -1)
                {
                    Console.WriteLine("Nome não encontrado. Você matou {0} gatinhos!", lPos[1]);
                }
                else
                {
                    Console.WriteLine("O nome se encontra na posição {0} e você matou {1} gatinhos", lPos[0], lPos[1]);
                }

                Console.WriteLine("Presione enter para continuar");
                
                Console.ReadKey();
            }
            
            getNames.Close();
        }
    }
}
