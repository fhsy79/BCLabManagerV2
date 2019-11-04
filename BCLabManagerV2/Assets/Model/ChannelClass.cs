﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChannelClass : AssetClass
    {
        public int Id { get; set; }
        //public TesterClass Tester { get; set; }
        //public String Name { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private TesterClass _tester;
        public TesterClass Tester
        {
            get { return _tester; }
            set { SetProperty(ref _tester, value); }
        }

        public ChannelClass()
        { }

        //public ChannelClass(String Name, TesterClass Tester)
        //{
        //    this.Tester = Tester;
        //    this.Name = Name;
        //}

        //public void Update(String Name, TesterClass Tester)
        //{
        //    this.Tester = Tester;
        //    this.Name = Name;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
