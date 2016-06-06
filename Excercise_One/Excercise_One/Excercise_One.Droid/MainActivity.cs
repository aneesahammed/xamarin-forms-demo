using System;
using Android.App;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using Exercise_One.Droid.Common;
using Exercise_One.Droid.Service;
using Android.Graphics;
using Android.Util;


namespace Excercise_One.Droid
{
	[Activity (Label = "Excercise_One.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity, View.IOnTouchListener
    {
        #region <-PrivateMembers->
        private int _x;
        private int _y;
        private int _xPos;
        private int _yPos;
        RelativeLayout _mainLayout;
        IRandomImageColorService _randomImageColorService;
        GestureDetector _gestureDetector;
        ImageView _selectedImageView;
        #endregion

        #region <-Constructor->
        public MainActivity()
        {
            _randomImageColorService = new RandomImageColorService();
        }
        #endregion



        #region <-ProtectedMethods->
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Set our view from the "main" layout resource
                SetContentView(Resource.Layout.Main);

                _mainLayout = FindViewById<RelativeLayout>(Resource.Id.mainLayout);

                _mainLayout.SetOnTouchListener(this);
            }
            catch (Exception ex)
            {
                Log.Error(Constants.APP, ex.Message);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        #endregion

        #region <-PublicMethods->
        public bool IsNetWorkAvailable()
        {
            try
            {
                var connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
                var activeConnection = connectivityManager.ActiveNetworkInfo;
                if ((activeConnection != null) && activeConnection.IsConnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.APP, ex.Message);
            }

            return false;
        }
        #endregion                

        #region <-Events->
        public bool OnTouch(View view, MotionEvent e)
        {
            switch (e.Action)
            {
                //--Down--
                case MotionEventActions.Down:
                    _x = (int)e.GetX();
                    _y = (int)e.GetY();

                    var layoutParams = new RelativeLayout.LayoutParams(
                        RelativeLayout.LayoutParams.WrapContent,
                        RelativeLayout.LayoutParams.WrapContent);

                    var imgView = new ImageView(this);
                    var size = (int)new Random().Next(100, 200);

                    var randomShape = Utility.GetRandomShape();

                    layoutParams.SetMargins(_x, _y, 0, 0);
                    imgView.LayoutParameters = layoutParams;
                    imgView.LayoutParameters.Width = size;
                    imgView.LayoutParameters.Height = size;

                    imgView.Tag = randomShape.ToString();

                    if (randomShape == Shape.Square)
                    {
                        //if-square                     
                        imgView.SetImageResource(Resource.Drawable.Square);

                    }
                    else if (randomShape == Shape.Circle)
                    {
                        //if-circle                       
                        imgView.SetImageResource(Resource.Drawable.Circle);
                    }


                    //sets background
                    SetShapeBackground(imgView, shape: randomShape);

                    //add to main-view
                    ((ViewGroup)view).AddView(imgView);

                    //OnTouchEvent
                    imgView.Touch += ImgView_Touch;

                    //detect double-tap
                    _gestureDetector = new GestureDetector(new GestureListener());
                    _gestureDetector.DoubleTap += GestureDetector_DoubleTap;

                    break;

                //--Move--
                case MotionEventActions.Move:
                    break;

                //--default--
                default:
                    return false;
            }

            return true;
        }



        private void ImgView_Touch(object sender, View.TouchEventArgs e)
        {
            try
            {
                _selectedImageView = ((ImageView)sender);
                _gestureDetector.OnTouchEvent(e.Event);

                var x = (int)e.Event.RawX;
                var y = (int)e.Event.RawY;

                switch (e.Event.Action)
                {
                    case MotionEventActions.Down:
                        var relLayoutParams = (RelativeLayout.LayoutParams)(((View)sender)).LayoutParameters;

                        _xPos = x - relLayoutParams.LeftMargin;
                        _yPos = y - relLayoutParams.TopMargin;
                        break;

                    case MotionEventActions.Move:
                        var layoutParams = (RelativeLayout.LayoutParams)((View)sender).LayoutParameters;

                        layoutParams.LeftMargin = x - _xPos;
                        layoutParams.TopMargin = y - _yPos;

                        layoutParams.RightMargin = -250;
                        layoutParams.BottomMargin = -250;

                        ((View)sender).LayoutParameters = layoutParams;
                        break;
                    case MotionEventActions.Up:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.APP, ex.Message);
            }
        }

        private void GestureDetector_DoubleTap(object sender, GestureDetector.DoubleTapEventArgs e)
        {
            if (_selectedImageView != null)
            {
                if (_selectedImageView.Tag + "" == Shape.Circle.ToString())
                {
                    SetShapeBackground(_selectedImageView, Shape.Circle);
                }
                else
                {
                    SetShapeBackground(_selectedImageView);
                }
            }
        }

        #endregion

        #region <-PrivateMethods->        
        /// <summary>
        /// sets background for shape passed.
        /// </summary>
        /// <param name="shapeImgView"></param>
        /// <param name="view"></param>
        /// <param name="shape"></param>

        private async void SetShapeBackground(ImageView shapeImgView, Shape shape = Shape.Square)
        {
            try
            {
                if (IsNetWorkAvailable())
                {
                    if (shape == Shape.Square)
                    {
                        Bitmap bitmap = await _randomImageColorService.GetRandomImagePattern
                                                                      (Constants.RANDOM_IMAGE_PATTERNS_API).ConfigureAwait(false);

                        if (bitmap != null)
                            shapeImgView.SetImageBitmap(bitmap);
                        else
                            SetRandomColorHex(shapeImgView);
                    }

                    if (shape == Shape.Circle)
                    {
                        var hexColor = _randomImageColorService.GetRandomColorHex(Constants.RANDOM_COLORS_API);
                        if (!String.IsNullOrEmpty(hexColor))
                        {
                            Drawable shapeBackground = shapeImgView.Drawable;

                            var color = string.Format("#{0}", hexColor);

                            ((GradientDrawable)shapeBackground).SetColor(Color.ParseColor(color));
                            ((GradientDrawable)shapeBackground).SetStroke(1, Color.ParseColor(color));
                        }
                        else
                            SetRandomColorHex(shapeImgView);
                    }
                }
                else
                {
                    //random hex for any shape
                    SetRandomColorHex(shapeImgView);
                }
            }
            catch (Exception ex)
            {
                Log.Error(Constants.APP, ex.Message);
            }
        }

        /// <summary>
        /// sets random color background to the shape passed.
        /// </summary>
        /// <param name="shapeImgView"></param>
        private void SetRandomColorHex(ImageView shapeImgView)
        {
            try
            {
                Drawable shapeBackground = shapeImgView.Drawable;
                ((GradientDrawable)shapeBackground).SetColor(Utility.GetRandomColor());
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}


