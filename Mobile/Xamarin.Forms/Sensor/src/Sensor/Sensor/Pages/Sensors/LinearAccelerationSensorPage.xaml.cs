﻿/*
* Copyright (c) 2017 Samsung Electronics Co., Ltd All Rights Reserved
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using Sensor.Pages.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sensor.Pages
{
    /// <summary>
    /// LinearAccelerationSensor Information Page
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinearAccelerationSensorPage : ContentPage
    {
        private Sensor.Models.LinearAccelerationSensorViewModel linearAccelerationSensor;
        private EventHandler<SensorEventArgs> listener;
        private SensorInfo info;

        /// <summary>
        /// Constructor of LinearAccelerationSensorPage
        /// </summary>
        /// <param name="info">Sensor Information</param>
        public LinearAccelerationSensorPage(SensorInfo info)
        {
            InitializeComponent();

            linearAccelerationSensor = new Sensor.Models.LinearAccelerationSensorViewModel();

            BindingContext = linearAccelerationSensor;
            this.FindByName<SensorInfoView>("SensorInfo").BindingContext = info;
            this.info = info;
        }

        /// <summary>
        /// Called when start button is clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        public void StartClicked(object sender, EventArgs args)
        {
            listener = (s, e) =>
            {
                linearAccelerationSensor.X = e.Values[0];
                linearAccelerationSensor.Y = e.Values[1];
                linearAccelerationSensor.Z = e.Values[2];
            };

            try
            {
                DependencyService.Get<ISensorManager>().StartSensor(info.Type, listener);
            }
            catch (Exception e)
            {
                DependencyService.Get<ILog>().Error(e.Message.ToString());
            }

            DependencyService.Get<ILog>().Info("Sensor Start");
        }

        /// <summary>
        /// Called when stop button is clicked.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        public void StopClicked(object sender, EventArgs args)
        {
            try
            {
                DependencyService.Get<ISensorManager>().StopSensor(info.Type, listener);
            }
            catch (Exception e)
            {
                DependencyService.Get<ILog>().Error(e.Message.ToString());
            }


            DependencyService.Get<ILog>().Info("Sensor Stop");
        }
    }
}
