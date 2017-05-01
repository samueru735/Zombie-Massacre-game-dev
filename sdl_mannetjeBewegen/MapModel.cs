using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2_ViewMapEditor
{
    class MapModel
    {
        StreamReader reader;
        //Constructors
        public MapModel(int breedte, int hoogte)
        {
            _map = new byte[hoogte, breedte];
        }
        public MapModel(byte[,] map)
        {

            _map = map;
        }
        public MapModel(string path)
        {
            try
            {
                this.LoadMap(path);
            }
            catch (SystemException e)
            {
                throw new SystemException(e.Message);
            }            
        }

        //Properties
        private byte[,] _map;
        public byte[,] Map
        {
            get
            {
                if (_map != null)
                    return _map;
                throw new NullReferenceException("Map not created");
            }
        }
        //ReadOnly Properties
        public int Hoogte
        {
            get { return _map.GetLength(0); }
        }
        public int Breedte
        {
            get { return _map.GetLength(1); }
        }

        //methods
        public void SetElement(int x, int y, int value)
        {//Done: check if valid x, y value
            try
            {
                _map[y, x] = (byte)value;
            }
            catch (IndexOutOfRangeException)
            {
                if (y > _map.GetLength(0))
                    y = _map.GetLength(0);
                if(x > _map.GetLength(1))
                    x = _map.GetLength(1);
            }             
        }
        public int GetElement(int x, int y)
        {
            try
            {
                return Convert.ToInt32(_map[y, x]);
            }
            catch (NullReferenceException) { throw new NullReferenceException("Waarde van opgegeven coördinaten bestaat niet"); }
            catch (FormatException) { throw new FormatException("Het gevraagde element is geen geldig formaat"); }

             //Done: check if valid x, y value
        }
        public void ClearMap()
        {
            for (int i = 0; i < Hoogte; i++)
            {
                for (int j = 0; j < Breedte; j++)
                {
                    _map[i, j] = 0;  //We could also just say _map= new byte[Hoogte, Breedte] ;
                }
            }
        }

        //FileIO
        public void LoadMap(string path)
        {
            try
            {
                reader = new StreamReader(path);
                int breedte = Convert.ToInt32(reader.ReadLine());
                int hoogte = Convert.ToInt32(reader.ReadLine());
                byte[,] resultaat = new byte[hoogte, breedte];
                for (int i = 0; i < hoogte; i++)
                {
                    //lees lijn per lijn
                    var lijn = reader.ReadLine();
                    //Splits komma's weg
                    var gesplitst = lijn.Split(',');
                    for (int j = 0; j < gesplitst.Length; j++) //Todo: controleren of Length overeenkomt met beloofde breedte aan begin file
                    {
                        resultaat[i, j] = (byte)Convert.ToInt32(gesplitst[j]);
                    }
                }
                _map = resultaat;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Meegegeven string is niet correct: " + path);
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("File niet gevonden op locatie: " + path);
            }
            catch (FormatException)
            {
                throw new FormatException("File " + path + " bevat verkeerde data");
            }
            finally { reader.Close(); }
        }

        public void SaveMap(string path)
        {
            //Klaar om te schrijven
            StreamWriter writer = new StreamWriter(path);
            //Mapdimensies schrijven 
            writer.WriteLine(Breedte);
            writer.WriteLine(Hoogte);

            for (int i = 0; i < Hoogte; i++)
            {
                for (int j = 0; j < Breedte; j++)
                {
                    writer.Write(_map[i,j]);
                    if (j < Breedte - 1)//Geen komma op einde van lijn
                        writer.Write(",");
                }
                writer.WriteLine();
            }
            writer.Close();
        }

        //ToString
        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < Hoogte; i++)
            {
                for (int j = 0; j < Breedte; j++)
                {
                    res += _map[i, j].ToString();
                }
                res += Environment.NewLine;
            }
            return res;
        }

    }
}
