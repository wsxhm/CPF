using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CPF;
using CPF.Drawing;
using CPF.Controls;
using CPF.Shapes;
using CPF.Styling;
using CPF.Animation;

namespace ConsoleApp1
{
    public class VideoPlayTest : Window
    {
        protected override void InitializeComponent()
        {
            Title = "标题";
            Width = 500;
            Height = 400;
            CanResize = true;
            Background = null;
            Children.Add(new WindowFrame(this, new Panel
            {
                Width = "100%",
                Height = "100%",
                Children = //内容元素放这里
                {
                    new VideoView
                    {
                        Name = nameof(player),
                        PresenterFor=this,
                        MarginBottom=0,
                        MarginLeft=0,
                        MarginRight=0,
                        MarginTop=0,
                    },
                    new Button
                    {
                        MarginLeft = 21.4f,
                        MarginTop = 10.1f,
                        Width = 73.2f,
                        Content = "播放",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>play()
                            }
                        }
                    },
                    new TextBox
                    {
                        Background = "#FFFFFF76",
                        MarginLeft = 122.1f,
                        MarginTop = 10.1f,
                        Width = 168.6f,
                        PresenterFor=this,
                        Name="textBox"
                    },
                    new Button
                    {
                        MarginLeft = 375f,
                        MarginTop = 13.1f,
                        Width = 73.2f,
                        Content = "暂停/播放",
                        Commands =
                        {
                            {
                                nameof(Button.Click),
                                (s,e)=>PauseOrPlay()
                            }
                        }
                    }
                }
            })
            {
                MaximizeBox = true,
            });
            LoadStyleFile("res://ConsoleApp1.Stylesheet1.css");
            //加载样式文件，文件需要设置为内嵌资源

            if (!DesignMode)//设计模式下不执行
            {
                
            }
        }
        VideoView player;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            //if (!DesignMode)
            //{
            //    var player = FindPresenterByName<VideoView>("player");
            //    player.Play(new Uri(@"https://bj.bcebos.com/kpy-organizations/U3017318/videos/2021091617080192065864213581.mp4"));
            //}
            player = FindPresenterByName<VideoView>(nameof(player));
        }

        async void play()
        {
            var player = FindPresenterByName<VideoView>("player");
            var textBox = FindPresenterByName<TextBox>("textBox");
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                var f = await new OpenFileDialog().ShowAsync(this);
                if (f != null && f.Length > 0)
                {
                    player.Play(new Uri(f[0]));
                }
            }
            else
            {
                player.Play(new Uri(textBox.Text));
            }

        }
        void PauseOrPlay()
        {
            var player = FindPresenterByName<VideoView>("player");
            if (player.MediaPlayer.IsPlaying)
            {
                player.MediaPlayer.Pause();
            }
            else
            {
                player.Play();
            }
        }

        protected override void OnDoubleClick(RoutedEventArgs e)
        {
            base.OnDoubleClick(e);
            player.MediaPlayer.ToggleFullscreen();
        }
    }
}
