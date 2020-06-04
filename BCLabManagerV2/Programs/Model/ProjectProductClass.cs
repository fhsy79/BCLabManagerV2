﻿using Prism.Mvvm;
namespace BCLabManager.Model
{
    public class ProjectProductClass : BindableBase
    {
        public int Id { get; set; }
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }
        private ProjectProductTypeClass _type;
        public ProjectProductTypeClass Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
    }
}