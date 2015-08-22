﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FileNotifier
{
    public class FileNotifierManager : IFileNotifierManager
    {
        private readonly IFileNotifier _fileNotifier;

        public FileNotifierManager(IFileNotifier fileNotifier)
        {
            _fileNotifier = fileNotifier;
            _observedFiles = new Dictionary<ObserveFileDto, IFileObserver>();
        }

        private readonly Dictionary<ObserveFileDto, IFileObserver> _observedFiles;

        public void Set(ObserveFileDto fileToObserve)
        {
            IFileObserver adapter = FileObserver.Create(fileToObserve, _fileNotifier);
            AddToObserverList(fileToObserve,adapter);
        }

        private void AddToObserverList(ObserveFileDto fileToObserve, IFileObserver adapter)
        {
            if (!_observedFiles.ContainsKey(fileToObserve))
            {
                adapter.Start();
                _observedFiles.Add(fileToObserve, adapter);
            }
            else
                throw new ArgumentException("This path was added to observable list before.");
        }

        public void Remove(string filePath)
        {
            try
            {
                var fileDto =
                    _observedFiles.Single(t => t.Key.Path.Equals(filePath, StringComparison.OrdinalIgnoreCase)).Key;
                _observedFiles.Remove(fileDto);
            }
            catch
            {
            }
        }

        public List<ObserveFileDto> PerformFileList()
        {
            return _observedFiles.Select(t => t.Key).ToList();
        }
    }
}