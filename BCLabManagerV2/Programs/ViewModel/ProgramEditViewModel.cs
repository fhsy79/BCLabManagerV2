﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class ProgramEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        public ProgramClass _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。
        ObservableCollection<BatteryTypeClass> _batteryTypes;
        RecipeTemplateViewModel _selectedRecipeTemplate;
        RecipeViewModel _selectedRecipe;
        RelayCommand _okCommand;
        RelayCommand _addCommand;
        RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProgramEditViewModel(
            ProgramClass programmodel,
            ObservableCollection<BatteryTypeClass> batteryTypes,
            ObservableCollection<RecipeTemplate> RecipeTemplates)
        {
            _program = programmodel;
            _batteryTypes = batteryTypes;
            this.CreateAllRecipeTemplates(RecipeTemplates);
            this.CreateRecipes();
        }


        void CreateAllRecipeTemplates(ObservableCollection<RecipeTemplate> RecipeTemplates)
        {
            List<RecipeTemplateViewModel> all =
                (from sub in RecipeTemplates
                 select new RecipeTemplateViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllRecipeTemplates = new ObservableCollection<RecipeTemplateViewModel>(all);     //再转换成Observable
        }
        void CreateRecipes()
        {
            List<RecipeViewModel> all =
                (from sub in _program.Recipes
                 select new RecipeViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (RecipeModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            this.Recipes = new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public int Id
        {
            get { return _program.Id; }
            set
            {
                if (value == _program.Id)
                    return;

                _program.Id = value;

                RaisePropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _program.Name; }
            set
            {
                if (value == _program.Name)
                    return;

                _program.Name = value;

                RaisePropertyChanged("Name");
            }
        }

        public BatteryTypeClass BatteryType       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _program.BatteryType;
            }
            set
            {
                if (value == _program.BatteryType)
                    return;

                _program.BatteryType = value;

                RaisePropertyChanged("BatteryType");
            }
        }

        public ObservableCollection<BatteryTypeClass> AllBatteryTypes //供选项
        {
            get
            {
                ObservableCollection<BatteryTypeClass> all = _batteryTypes;

                return new ObservableCollection<BatteryTypeClass>(all);
            }
        }
        public string Requester
        {
            get { return _program.Requester; }
            set
            {
                if (value == _program.Requester)
                    return;

                _program.Requester = value;

                RaisePropertyChanged("Requester");
            }
        }

        public string Description
        {
            get { return _program.Description; }
            set
            {
                if (value == _program.Description)
                    return;

                _program.Description = value;

                RaisePropertyChanged("Description");
            }
        }

        public DateTime RequestDate
        {
            get { return _program.RequestTime; }
            set
            {
                if (value == _program.RequestTime)
                    return;

                _program.RequestTime = value;

                RaisePropertyChanged("RequestDate");
            }
        }

        public ObservableCollection<RecipeViewModel> Recipes { get; set; }        //这个是当前program所拥有的Recipes
        /*{
            get
            {
                if (_program.Recipes == null)
                    return new ObservableCollection<RecipeViewModel>();
                List<RecipeViewModel> all =
                    (from bat in _program.Recipes
                     select new RecipeViewModel(bat, _RecipeRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (RecipeModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

                return new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            }
        }*/

        #endregion // Customer Properties

        #region Presentation Properties

        public ObservableCollection<RecipeTemplateViewModel> AllRecipeTemplates { get; private set; }   //展示所有RecipeTemplate以便选用,跟Recipes是不一样的

        public RecipeTemplateViewModel SelectedRecipeTemplate
        {
            get
            {
                return _selectedRecipeTemplate;
            }
            set
            {
                if (_selectedRecipeTemplate != value)
                {
                    _selectedRecipeTemplate = value;
                }
            }
        }

        public RecipeViewModel SelectedRecipe
        {
            get
            {
                return _selectedRecipe;
            }
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                }
            }
        }
        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => { this.Add(); },
                        param => this.CanAdd
                            );
                }
                return _addCommand;
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => { this.Remove(); },
                        param => this.CanRemove
                            );
                }
                return _removeCommand;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            foreach (var sub in _program.Recipes)
            {
                foreach (var tr in sub.TestRecords)
                    tr.ProgramStr = this.Name;
            }
            IsOK = true;
        }

        public void Add()       //对于model来说，需要将选中的sub copy到_program.Recipes来。对于viewmodel来说，需要将这个copy出来的sub，包装成viewmodel并添加到this.Recipes里面去
        {
            var newsubmodel = new RecipeClass(SelectedRecipeTemplate._recipeTemplate, BatteryType);
            var newsubviewmodel = new RecipeViewModel(newsubmodel);
            _program.Recipes.Add(newsubmodel);
            this.Recipes.Add(newsubviewmodel);
        }

        public void Remove()       //对于model来说，需要将选中的sub 从_program.Recipes中移除。对于viewmodel来说，需要将这个viewmodel从this.Recipes中移除
        {
            _program.Recipes.Remove(SelectedRecipe._recipe);
            this.Recipes.Remove(SelectedRecipe);
        }

        //public ProgramViewModel Clone()
        //{
        //    return new ProgramViewModel(_program.Clone(), _programRepository, _RecipeRepository);
        //}

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewProgram
        {
            get
            {
                var dbContext = new AppDbContext();
                int number = (
                    from bat in dbContext.Programs
                    where bat.Name == _program.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewProgram; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProgram; }
        }

        bool CanAdd
        {
            get { return SelectedRecipeTemplate!=null; }
        }

        bool CanRemove
        {
            get { return SelectedRecipe != null; }     //如果已经有数据，可否删除？
        }

        #endregion // Private Helpers
    }
}