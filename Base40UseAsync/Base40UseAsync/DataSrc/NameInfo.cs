using System;
using System.Threading.Tasks;

namespace Fjc.DataSrc
{
    public class NameInfo
    {
        public NameInfo(int id,string name)
        {
            Index = id;
            _Name = name;
        }
        public NameInfo(int id,Task<string> tname)
        {
            Index = id;
            tName = tname;
        }
        public int Index { get; set; }
        private string _Name;
        public string Name 
        {
            get{ return _Name ?? tName.Result;}
            set{ _Name = value;} 
        }
        public Task<string> tName { get; set; }
        public override string ToString()
        {
            return string.Format("Inde:{0:d3},Name: {1}",Index,Name);
        }
    }
}

