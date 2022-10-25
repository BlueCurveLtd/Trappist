using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using LottieSharp;

using SkiaSharp;

namespace Trappist.Wpf.Bedrock.Controls.Animations;

[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
public class LottieImage : Image
{
    private Int32Rect? rectangle;
    private WriteableBitmap? bitmap;
    private SKSurface? surface;
    private LottieDrawable? lottieDrawable;
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(LottieImage), new PropertyMetadata(null));

    /// <summary>
    /// The animation relative path.
    /// </summary>
    public string? FileName
    {
        get => this.GetValue(FileNameProperty) as string;
        set => this.SetValue(FileNameProperty, value);
    }

    public static readonly DependencyProperty AutoPlayProperty = DependencyProperty.Register(nameof(AutoPlay), typeof(bool), typeof(LottieImage), new PropertyMetadata(true));

    /// <summary>
    /// Enables or disables autoplay.
    /// </summary>
    /// <remarks>Default is <c>true</c>.</remarks>
    public bool AutoPlay
    {
        get => (bool)this.GetValue(AutoPlayProperty);
        set => this.SetValue(AutoPlayProperty, value);
    }

    public static readonly DependencyProperty RepeatModeProperty = DependencyProperty.Register(nameof(RepeatMode), typeof(LottieSharp.RepeatMode), typeof(LottieImage), new PropertyMetadata(LottieSharp.RepeatMode.Restart));

    /// <summary>
    /// The repeat mode.
    /// </summary>
    /// <remarks>Default is <c>Restart</c>.</remarks>
    public LottieSharp.RepeatMode RepeatMode
    {
        get => (LottieSharp.RepeatMode)this.GetValue(RepeatModeProperty);
        set => this.SetValue(RepeatModeProperty, value);
    }


    public static readonly DependencyProperty RepeatCountProperty = DependencyProperty.Register(nameof(RepeatCount), typeof(int), typeof(LottieImage), new PropertyMetadata(LottieDrawable.Infinite));

    /// <summary>
    /// The repeat count.
    /// </summary>
    /// <remarks>Default is <c>LottieDrawable.Infinite</c> (-1).</remarks>
    public int RepeatCount
    {
        get => (int)this.GetValue(RepeatCountProperty);
        set => this.SetValue(RepeatCountProperty, value);
    }

    public LottieImage()
    {
        this.Loaded += this.LottieImage_Loaded;
        this.Unloaded += this.LottieImage_Unloaded;
    }

    private void LottieImage_Unloaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= this.LottieImage_Loaded;
        this.Unloaded -= this.LottieImage_Unloaded;

        this.lottieDrawable?.Stop();
        this.lottieDrawable?.Dispose();
    }

    private void LottieImage_Loaded(object sender, RoutedEventArgs e)
    {
        this.rectangle = new Int32Rect(0, 0, (int)this.Width, (int)this.Height);
        this.bitmap = new WriteableBitmap((int)this.Width, (int)this.Height, 96.0D, 96.0D, PixelFormats.Bgra32, null);
        
        this.surface = SKSurface.Create(new SKImageInfo((int)this.Width, (int)this.Height, SKColorType.Bgra8888, SKAlphaType.Premul), pixels: this.bitmap.BackBuffer, rowBytes: (int)(this.Width * 4D));
   
        this.Source = this.bitmap;

        this.InitLottieRenderer();
    }

    private void InitLottieRenderer()
    {
        this.lottieDrawable = new LottieDrawable();
        var result = LottieCompositionFactory.FromJsonStringSync(
            File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, this.FileName!), Encoding.UTF8),
            System.IO.Path.GetFileNameWithoutExtension(this.FileName!));
         
        this.lottieDrawable.SetComposition(result.Value);
        this.lottieDrawable.RepeatCount = this.RepeatCount;
        this.lottieDrawable.RecycleBitmaps();
        this.lottieDrawable.RepeatMode = this.RepeatMode;

        this.lottieDrawable.DirectScale = (float)this.ComputeScale(this.lottieDrawable.Size);
        this.lottieDrawable.EnableMergePaths(true); 
        this.lottieDrawable.AnimatorUpdate += this.Drawable_AnimatorUpdate;
        this.lottieDrawable.Start();
    }

    private double ComputeScale(SKSize originalSize)
    {
        var widthRatio = this.Width / (double)originalSize.Width;
        var heightRatio = this.Height / (double)originalSize.Height;
        var minAspectRatio = Math.Min(widthRatio, heightRatio);

        return minAspectRatio;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD001:Avoid legacy thread switching APIs", Justification = "Wpf")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S108:Nested blocks of code should not be left empty", Justification = "Wpf")]
    private void Drawable_AnimatorUpdate(object? sender, ValueAnimator.ValueAnimatorUpdateEventArgs e)
    {
        try
        {
            this.bitmap!.Dispatcher!.Invoke(() =>
            {
                bool isLocked = false;

                try
                {
                    this.bitmap.Lock();
                    isLocked = true;

                    this.surface!.Canvas.Clear();

                    this.lottieDrawable!.Draw(this.surface!);



                    this.bitmap.AddDirtyRect(this.rectangle!.Value);
                }
                finally
                {
                    if (isLocked)
                    {
                        this.bitmap.Unlock();
                    }
                }
            });
        }
        catch (TaskCanceledException)
        { 
        }
    }
}
