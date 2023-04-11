// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.WindowManagement;
using ExcelDataReader;
using System.Data;
using Microsoft.UI.Xaml.Documents;
using Newtonsoft.Json;
using Windows.UI.ViewManagement;
using System.Security.Cryptography.X509Certificates;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;
using System.Drawing;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Artifactor
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            //Set the tilte bar
            /*Title = "Artifactor Tool";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);*/

            //Change the default launch size of the window
            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(500, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            //checkList.Add(checks[5].testName);
            

        }

        public string OutputFolder = "";
        public string json = "";
        public List<Checks> checks = new List<Checks>();
        public ObservableCollection<Checks> checksSanitized = new ObservableCollection<Checks>();
        //public ObservableCollection<String> checkList = new ObservableCollection<string>();

        private void excelToJson(object sender, RoutedEventArgs e)
        {
                        
            using (var stream = File.Open("C:\\Users\\jsheb\\Downloads\\Deloitte_Allianz_IAPT_Checklist_v.1.2.xlsx", FileMode.Open, FileAccess.Read)) 

            {
                //Had to include to stop the reader from breaking coz not supporting encoding
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);


                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Choose one of either 1 or 2:

                    // 1. Use the reader methods
                    do
                    {

                        if (reader.Name == "WebApp")
                        {
                            while (reader.Read())
                            {
                                checks.Add(new Checks()
                                {
                                    testID = reader.GetString(0),
                                    testName = reader.GetString(1),
                                    testDescription = reader.GetString(2),
                                    testType = reader.GetString(3),
                                });

                            }
                        }
                    } while (reader.NextResult());

                    for (int i = 0; i < checks.Count; i++)
                    {
                        if (checks[i].testName != null)
                        {
                            checksSanitized.Add(checks[i]);
                        }
                    }


                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet().Tables["WebApp"];


                    //Serialize the Checks list object to json string
                    json = JsonConvert.SerializeObject(checks, Formatting.Indented);


                    // The result of each spreadsheet is in result.Tables


                    var xmloutputFile = File.OpenWrite("C:\\Users\\jsheb\\Downloads\\xmlOutput.txt");
                    result.WriteXml(xmloutputFile);

                    //var jsonoutputFile = File.OpenWrite("C:\\Users\\jsheb\\Downloads\\jsonOutput.txt");
                    File.WriteAllText(@"C:\Users\jsheb\Downloads\jsonOutput.txt", json);


                }

            }
        }



        private async void PickFolderButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickFolderOutputTextBlock.Text = "";

            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            // var window = WindowHelper.GetAppWindow(this);
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                PickFolderOutputTextBlock.Text = "Picked folder: " + folder.Path;

            }
            else
            {
                PickFolderOutputTextBlock.Text = "Operation cancelled.";
            }
        }

        
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (json!=null && OutputFolder!=null)
            {
                for (int i = 0; i < checks.Count(); i++)
                {
                    if (checks[i].testName != null) {
                        checksSanitized.Add(checks[i]);
                    }
                    
                }
            }

        }

        private async void Paste_Click(object sender, RoutedEventArgs e)
        {
            
            Checks checkPaste = (sender as FrameworkElement).DataContext as Checks;
            if (checkPaste.testID == null)
                consoleLog.Text = "Invalid Index";
            else
                consoleLog.Text = checkPaste.testID.ToString();

            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView != null && dataPackageView.Contains("Bitmap"))
            {
                IRandomAccessStreamReference imageReceived = null;
                imageReceived = await dataPackageView.GetBitmapAsync();
                if (imageReceived != null)
                {
                    using (var imageStream = await imageReceived.OpenReadAsync())
                    {
                        /*WriteableBitmap bitmapImage = new WriteableBitmap(500, 500);
                        await bitmapImage.SetSourceAsync(imageStream);

                        StorageFile outputFile = new StorageFile.GetFileFromPathAsync("C:\\Users\\jsheb\\Downloads");


                        FileStream imageFileStream = File.OpenWrite(OutputFolder + "\\" + checks[0].testID + "_001");
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, imageFileStream);*/

                        var fileSave = new FileSavePicker();
                        fileSave.FileTypeChoices.Add("Image", new string[] { ".png" });
                        fileSave.DefaultFileExtension = ".png";
                        fileSave.SuggestedFileName = checkPaste.testID + "_001";
                        fileSave.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

                        // Retrieve the window handle (HWND) of the current WinUI 3 window. 
                        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

                        // Initialize the folder picker with the window handle (HWND).
                        WinRT.Interop.InitializeWithWindow.Initialize(fileSave, hWnd);

                        var storageFile = await fileSave.PickSaveFileAsync();

                        checksSanitized.GetEnumerator().MoveNext();
                        //TODO: Create a null exception

                        using (var stream = await storageFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await imageStream.AsStreamForRead().CopyToAsync(stream.AsStreamForWrite());
                        }

                    }
                }
            }

            else { Paste_Click(sender, e);}
        }
    }
}
