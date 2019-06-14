﻿using Microsoft.PythonTools.Interpreter;
using Microsoft.PythonTools.Projects;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudioTools.TestAdapter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.PythonTools.TestAdapter.Model {
    internal class ProjectInfo : IDisposable {
        private readonly PythonProject _pythonProject;
        private readonly string _projectHome;
        private readonly TestContainerDiscoverer _discoverer;
        private readonly Dictionary<string, TestContainer> _containers;

        public ProjectInfo(TestContainerDiscoverer discoverer, PythonProject project) {
            _pythonProject = project;
            _projectHome = _pythonProject.ProjectHome;
            _discoverer = discoverer;
            _containers = new Dictionary<string, TestContainer>(StringComparer.OrdinalIgnoreCase);
        }

        public void Dispose() {
        }

        public TestContainer[] GetAllContainers() {
            return _containers.Select(x => x.Value).ToArray();
        }

        public bool TryGetContainer(string path, out TestContainer container) {
            return _containers.TryGetValue(path, out container);
        }

        public LaunchConfiguration GetLaunchConfigurationOrThrow() {
            return _pythonProject.GetLaunchConfigurationOrThrow();
        }

        public string GetProperty(string name) {
            return _pythonProject.GetProperty(name);
        }


        public void AddTestContainer(string path) {

            TestContainer existing;
            if (!TryGetContainer(path, out existing)) {
            
                _containers[path] = new TestContainer(
                    _discoverer,
                    path,
                    _projectHome,
                    version:0,
                    Architecture,
                    null
                );
            } 
            else {
                RemoveTestContainer(path);

                _containers[path] = new TestContainer(
                   _discoverer,
                   path,
                   _projectHome,
                   version: existing.Version + 1,
                   Architecture,
                   null
               );
            }
        }

        public bool RemoveTestContainer(string path) {
            return _containers.Remove(path);
        }

        private Architecture Architecture => Architecture.Default;
    }
}
