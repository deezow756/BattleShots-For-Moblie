using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Xamarin.Forms;

namespace BattleShots.Droid
{
    [Activity(Label = "BattleShots", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            BGData.activity = this;

            const int locationPermissionsRequestCode = 1000;
            const int storagePermissionsRequestCode = 500;

            var locationPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation
            };

            var StoragePermissions = new[]
            {
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage
            };

            // check if the app has permission to access coarse location
            var coarseLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

            // check if the app has permission to access fine location
            var fineLocationPermissionGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            // if either is denied permission, request permission from the user
            if (coarseLocationPermissionGranted == Permission.Denied ||
                fineLocationPermissionGranted == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);
            }

            var readStoragePermissonGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage);

            var writeStoragePermissonGranted =
                ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);

            if(readStoragePermissonGranted == Permission.Denied || writeStoragePermissonGranted == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, StoragePermissions, storagePermissionsRequestCode);
            }

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 2)
            {
                if (resultCode != Result.Ok)
                {
                    ToastLoader toastLoader = new ToastLoader();
                    toastLoader.Show("Bluetooth Must Be Enabled To Play Game");
                    BGData.btManager.BtBeingEnabled = false;
                    BGData.btManager.TryEnableBluetooth();
                }
                else
                {
                    BGData.btManager.BtBeingEnabled = false;
                    if (BGStuff.settingUpGame == false && BGStuff.Reconnecting == false)
                    {
                        BGStuff.mainPage.GetKnownDevices();
                        BGStuff.mainPage.StartScan();
                        BGData.btManager.ReceivingConnection = true;
                        BGData.btManager.ReceivePair();
                    }                    
                }
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}