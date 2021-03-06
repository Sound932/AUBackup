﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using MVVMAUbackup.Models;
using MVVMAUbackup.Commands;
using System.Runtime.Serialization;

namespace MVVMAUbackup.ViewModels
{
    [Serializable]
    class StatusViewModel : ISerializable
    {
        
        #region Constructor
        public StatusViewModel()
        {
            _statuses = new ObservableCollection<StatusModel>()
            {
                new StatusModel{
                    ElapsedTime = FolderViewModel.BackupTimer.Interval,
                    DateFinished = null,
                    BackupStatus = nameof(BackupProgress.NotStarted) 
                }
            };
            _updateProgress = new DispatcherTimer();
            _updateProgress.Interval = TimeSpan.FromSeconds(1);
            _updateProgress.Tick += UpdateElapsedTime;
        }
        #endregion

        #region Fields
        private ObservableCollection<StatusModel> _statuses;
        private DispatcherTimer _updateProgress;
        #endregion


        #region Properties       
        public ObservableCollection<StatusModel> Statuses => _statuses;
        public DispatcherTimer UpdateProgress => _updateProgress;
        #endregion

        #region Methods
        private void AddStatus()
        {
            _statuses.Insert(0,
                 new StatusModel
                 {
                     ElapsedTime = FolderViewModel.BackupTimer.Interval,
                     DateFinished = null,
                     BackupStatus = nameof(BackupProgress.InProgress)
                 });
        }
        private void UpdateElapsedTime(object sender, EventArgs e)
        {
            if(_statuses[0].ElapsedTime != TimeSpan.FromSeconds(0))
            {
                _statuses[0].ElapsedTime -= TimeSpan.FromSeconds(1);
                return;
            }
            UpdateFinishedProcess();
        }

        public void PauseProcess()
        {
            _updateProgress.Stop();
            _statuses[0].BackupStatus = nameof(BackupProgress.Paused);
        }
        public void StartProcess()
        {
            _updateProgress.Start();
            _statuses[0].BackupStatus = nameof(BackupProgress.InProgress);
        }

        public void UpdateFinishedProcess()
        {
            _statuses[0].DateFinished = DateTime.Now;
            _statuses[0].BackupStatus = nameof(BackupProgress.Finished);
            AddStatus();
        }
        #endregion


        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Statuses", _statuses);
        }

        public StatusViewModel(SerializationInfo info, StreamingContext context) : this()
        {
            _statuses = (ObservableCollection<StatusModel>)info.GetValue("Statuses", typeof(ObservableCollection<StatusModel>));

        }
        #endregion
    }
}
