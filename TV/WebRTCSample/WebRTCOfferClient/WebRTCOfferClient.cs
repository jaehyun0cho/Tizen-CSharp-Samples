﻿using System.Drawing;
using System.Reflection.Metadata;
/*
* Copyright (c) 2021 Samsung Electronics Co., Ltd All Rights Reserved
*
* Licensed under the Apache License, Version 2.0 (the License);
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an AS IS BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using Tizen.System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace WebRTCOfferClient
{
    public class Program : NUIApplication
    {
        private ConnectionManager connectionManager;

        protected override void OnCreate()
        {
            base.OnCreate();
            Initialize();
        }

        public void OnKeyEvent(object sender, Window.KeyEventArgs e)
        {
            if (e.Key.State == Key.StateType.Down && (e.Key.KeyPressedName == "XF86Back" || e.Key.KeyPressedName == "Escape"))
            {
                connectionManager.Dispose();
                Exit();
            }
        }

        private void Initialize()
        {
            connectionManager = new ConnectionManager();
            connectionManager.PreviewResolution = new Tizen.Multimedia.Size(
                UIParam.PreviewResolution.Width, UIParam.PreviewResolution.Height);

            //FIXME:need to change this to user input
            connectionManager.SetMediaSourceType(SourceType.MediaPacket);
            connectionManager.TransceiverDirection = Tizen.Multimedia.Remoting.TransceiverDirection.SendRecv;

            Window window = Window.Instance;
            window.BackgroundColor = Tizen.NUI.Color.White;
            window.KeyEvent += OnKeyEvent;

            MainPage page = new MainPage();
            page.PositionUsesPivotPoint = true;
            page.ParentOrigin = ParentOrigin.Center;
            page.PivotPoint = PivotPoint.Center;
            page.Size = new Tizen.NUI.Size(window.WindowSize.Width, window.WindowSize.Height, 0);
            window.Add(page);

            var remotePeerIdText = new TextField()
            {
                PlaceholderText="Enter remote peer Id",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                PointSize = UIParam.FontSize,
                Position2D =  UIParam.Position1R1C,
                Size2D = UIParam.Size1R1C,
                CursorWidth = 10,
                EnableCursorBlink = true
            };
            page.Add(remotePeerIdText);

            var connectButton = new Button()
            {
                HeightResizePolicy = ResizePolicyType.FillToParent,
                Text = "Connect",
                //Weight = 1,
                PointSize = UIParam.FontSize,
                Position2D =  UIParam.Position1R2C,
                Size2D = UIParam.Size1R2C,
            };
            page.Add(connectButton);

            var resetButton = new Button()
            {
                HeightResizePolicy = ResizePolicyType.FillToParent,
                Text = "Reset",
                //Weight = 1,
                PointSize = UIParam.FontSize,
                Position2D =  UIParam.Position1R3C,
                Size2D = UIParam.Size1R3C,
                IsEnabled = false
            };
            page.Add(resetButton);

            var previewLabel = new TextLabel()
            {
                Text = "[ Local preview ]",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                PointSize = UIParam.FontSize,
                Position2D =  UIParam.Position2R1C,
                Size2D = UIParam.Size2R1C,
            };
            page.Add(previewLabel);

            var cameraView = new CameraView(connectionManager.GetCameraHandle())
            {
                Position2D =  UIParam.Position3R1C,
                Size2D = UIParam.Size3R1C,
            };
            page.Add(cameraView);

            if (UIParam.IsSupportedMultiWindow &&
                connectionManager.TransceiverDirection == Tizen.Multimedia.Remoting.TransceiverDirection.SendRecv)
            {
                var remoteviewLabel = new TextLabel()
                {
                    Text = "[ Video from remote ]",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    PointSize = UIParam.FontSize,
                    Position2D =  UIParam.Position2R2C,
                    Size2D = UIParam.Size2R2C,
                };

                var remoteView = new Window(new Tizen.NUI.Rectangle(UIParam.Position3R2C.X, UIParam.Position3R2C.Y,
                                                                    UIParam.Size3R2C.Width, UIParam.Size3R2C.Height))
                {
                    BackgroundColor = Tizen.NUI.Color.Transparent,
                    Title = "RemoteView"
                };

                page.Add(remoteviewLabel);
                connectionManager.SetRemoteView(remoteView);
            }

            var chatMessageText = new TextEditor();
            if (UIParam.IsSupportedMultiWindow)
            {
                chatMessageText.Position2D =  UIParam.Position4R1C;
                chatMessageText.Size2D = UIParam.Size4R1C;
                chatMessageText.BackgroundColor = Tizen.NUI.Color.LightGray;
            }
            else
            {
                chatMessageText.Position2D =  UIParam.Position4R1C2;
                chatMessageText.Size2D = UIParam.Size4R1C2;
                chatMessageText.BackgroundColor = Tizen.NUI.Color.LightGray;
            }
            page.Add(chatMessageText);

            remotePeerIdText.TextChanged += (s, e) =>
                Tizen.Log.Info(WebRTCLog.Tag, $"Enter,{e.TextField.Text}");

            connectButton.Clicked += (s, e) =>
            {
                connectButton.IsEnabled = false;
                resetButton.IsEnabled = true;

                if (remotePeerIdText.Text != null)
                {
                    try
                    {
                        connectionManager.Connect(Int32.Parse(remotePeerIdText.Text));
                    }
                    catch
                    {
                        Tizen.Log.Error(WebRTCLog.Tag,
                            "Failed to connect to remote peer. Please check peer id and try again");
                    }
                }
            };

            resetButton.Clicked += (s, e) =>
            {
                Tizen.Log.Info(WebRTCLog.Tag, "Reset.");
                resetButton.IsEnabled = false;

                connectionManager.Disconnect();

                remotePeerIdText.Text = "";
                connectButton.IsEnabled = true;
            };
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }

        internal static class UIParam
        {
            private static readonly string mobileProfile = "mobile";
            private static string Profile {get; set;} = "common";
            internal static bool IsSupportedMultiWindow {get; private set;} = true;
            internal static int FontSize {get; private set;} = 20;
            internal static Size2D PreviewResolution {get; private set;} = new Size2D(640, 480);
            internal static int Margin {get; private set; } = 30;

            internal static Size2D WindowResolution {get; private set;} =
                new Size2D(PreviewResolution.Width, PreviewResolution.Height);

            internal static int ButtonTextHeight = 50;

            // RemotePeerId TextField
            internal static Position2D Position1R1C = new Position2D(Margin, Margin);
            internal static Size2D Size1R1C = new Size2D(200, ButtonTextHeight);

            // Connect button
            internal static Position2D Position1R2C = new Position2D(Position1R1C.X + Size1R1C.Width + Margin, Margin);
            internal static Size2D Size1R2C = new Size2D(150, ButtonTextHeight);

            // Reset button
            internal static Position2D Position1R3C = new Position2D(Position1R2C.X + Size1R2C.Width + Margin, Margin);

            internal static Size2D Size1R3C = new Size2D(150, ButtonTextHeight);

            // Local preview label
            internal static Position2D Position2R1C = new Position2D(Position1R1C.X, Position1R1C.Y + Size1R1C.Height + Margin);
            internal static Size2D Size2R1C = new Size2D(PreviewResolution.Width, ButtonTextHeight);

            // Remote video label
            internal static Position2D Position2R2C = new Position2D(Position2R1C.X + Size2R1C.Width + Margin, Position2R1C.Y);
            internal static Size2D Size2R2C = new Size2D(PreviewResolution.Width, ButtonTextHeight);

            // Local preview view
            internal static Position2D Position3R1C = new Position2D(Position2R1C.X, Position2R1C.Y + Size2R1C.Height + Margin);
            internal static Size2D Size3R1C = new Size2D(PreviewResolution.Width, PreviewResolution.Height);

            // Remote video view
            internal static Position2D Position3R2C = new Position2D(Position3R1C.X + Size3R1C.Width + Margin, Position3R1C.Y);
            internal static Size2D Size3R2C = new Size2D(PreviewResolution.Width, PreviewResolution.Height);

            // Chat field
            internal static Position2D Position4R1C = new Position2D(Position3R1C.X, Position3R1C.Y + Size3R1C.Height + Margin);
            internal static Size2D Size4R1C = new Size2D(PreviewResolution.Width * 2 + Margin, 300);

            internal static Position2D Position4R1C2 = new Position2D(Position3R1C.X, Position3R1C.Y + Size3R1C.Height + Margin);
            internal static Size2D Size4R1C2 = new Size2D(PreviewResolution.Width, 300);


            static UIParam()
            {
                Tizen.Log.Info(WebRTCLog.Tag, "enter");
                if (!Information.TryGetValue("tizen.org/feature/profile", out string profile))
                {
                    throw new InvalidOperationException("Failed to get profile");
                }

                Profile = profile;

                Initialize();
            }

            private static void Initialize()
            {
                Tizen.Log.Info(WebRTCLog.Tag, "enter");
                if (string.Compare(Profile, mobileProfile) == 0)
                {
                    Tizen.Log.Info(WebRTCLog.Tag, "mobile");
                    FontSize = 6;
                    IsSupportedMultiWindow = false;
                    PreviewResolution = new Size2D(320, 240);
                    Margin = 20;
                }
            }
        }
    }
}