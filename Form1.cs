using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WhiteBee_Beta_v1._0
{
    public partial class Form1 : Form
    {

        //
        // Начало на дизайн кода
        //

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
        );
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        private bool aeroEnabled;
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;
        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        protected override CreateParams CreateParams
        {
            get
            {
                aeroEnabled = CheckAeroEnabled();

                CreateParams cp = base.CreateParams;
                if (!aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW;

                return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0;
                DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private static Rectangle RTop, RLeft, RBottom, RRight, RTopLeft, RTopRight, RBottomLeft, RBottomRight;
        private void MenuPanel_Resize(object sender, EventArgs e)
        {
            MenuButton.Location = new Point(50, BorderPanel.Height + MenuPanel.Height - 1);
        }
        private void MenuButton_Click(object sender, EventArgs e)
        {
            ControlLayer.Focus();
            MenuAnimation.Panel = MenuPanel;
            MenuAnimation.Animate();
            MenuButton.FlatAppearance.BorderSize = MenuAnimation.Visible ? 1 : 0;
        }
        private void ShadowPanel_Resize(object sender, EventArgs e)
        {
            LShadow.Width = MenuButton.Location.X + 1;
            LShadow.Dock = DockStyle.Left;
            RShadow.Width = ShadowPanel.Width - MenuButton.Width - MenuButton.Location.X + 1;
            RShadow.Dock = DockStyle.Right;
            ShadowPanel.Refresh();
        }
        private void MenuPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void MenuButton_MouseClick(object sender, MouseEventArgs e)
        {
            ControlLayer.Focus();
        }
        private void MenuButton_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized) this.WindowState = FormWindowState.Normal;
            else this.WindowState = FormWindowState.Maximized;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Minimized;
        }
        private void button4_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ImageLayers.MoveLayerFront(ImageSelect.Layer);
            ControlLayer.Focus();
        }
        private void button5_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ImageLayers.MoveLayerBack(ImageSelect.Layer);
            ControlLayer.Focus();
        }

        private void button6_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }
        private void button7_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }
        private void button8_Enter(object sender, EventArgs e)
        {
            (sender as Button).Parent.Focus();
        }
        private const int HTLEFT = 10,
                          HTRIGHT = 11,
                          HTTOP = 12,
                          HTTOPLEFT = 13,
                          HTTOPRIGHT = 14,
                          HTBOTTOM = 15,
                          HTBOTTOMLEFT = 16,
                          HTBOTTOMRIGHT = 17;

        const int borderPadding = 2;
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_NCPAINT && aeroEnabled)
            {
                var v = 2;
                DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                MARGINS margins = new MARGINS()
                {
                    bottomHeight = 1,
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 0
                };
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }

            if (message.Msg == WM_NCHITTEST && this.WindowState != FormWindowState.Maximized) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (RBottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (RBottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (RLeft.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (RRight.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (RBottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ImageLayersContainer.Location = new Point(borderPadding, BorderPanel.Height);
            ImageLayersContainer.Width = this.Width - 2 * borderPadding;
            ImageLayersContainer.Height = this.Height - borderPadding - BorderPanel.Height;
            TextLayerContainer.Size = ImageLayersContainer.Size;
            PenLayerContainer.Size = ImageLayersContainer.Size;
            PresentationLayerContainer.Size = ImageLayersContainer.Size;
            ControlLayer.Size = ImageLayersContainer.Size;
            RTop = new Rectangle(0, 0, this.ClientSize.Width, borderPadding);
            RLeft = new Rectangle(0, 0, borderPadding, this.ClientSize.Height);
            RBottom = new Rectangle(0, this.ClientSize.Height - borderPadding, this.ClientSize.Width, borderPadding);
            RRight = new Rectangle(this.ClientSize.Width - borderPadding, 0, borderPadding, this.ClientSize.Height);
            RTopLeft = new Rectangle(0, 0, borderPadding, borderPadding);
            RTopRight = new Rectangle(this.ClientSize.Width - borderPadding, 0, borderPadding, borderPadding);
            RBottomLeft = new Rectangle(0, this.ClientSize.Height - borderPadding, borderPadding, borderPadding);
            RBottomRight = new Rectangle(this.ClientSize.Width - borderPadding, this.ClientSize.Height - borderPadding, borderPadding, borderPadding);
        }

        private void button3_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }

        private void button2_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            ControlLayer.Focus();
        }

        //
        // Край на дизайн кода
        //

        public Form1()
        {
            // Зарежда формата
            InitializeComponent();
            this.DoubleBuffered = true;
            Size FullSize = Screen.PrimaryScreen.Bounds.Size;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimumSize = new Size(300, 200);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.Size = new Size(FullSize.Width * 3 / 4, FullSize.Height * 7 / 8);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;

            // Задава йерархията на слоевете. Отдолу нагоре: картинки -> текст -> писалка -> контролен слой
            TextLayerContainer.Parent = ImageLayersContainer;
            PenLayerContainer.Parent = TextLayerContainer;
            PresentationLayerContainer.Parent = PenLayerContainer;
            ControlLayer.Parent = PresentationLayerContainer;
            ImageLayersContainer.Location = new Point(borderPadding, BorderPanel.Height);
            ImageLayersContainer.Width = this.Width - 2 * borderPadding;
            ImageLayersContainer.Height = this.Height - borderPadding - BorderPanel.Height;
            TextLayerContainer.Location = new Point(0, 0);
            PenLayerContainer.Location = new Point(0, 0);
            PresentationLayerContainer.Location = new Point(0, 0);
            ControlLayer.Location = new Point(0, 0);
            TextLayerContainer.Size = ImageLayersContainer.Size;
            PenLayerContainer.Size = ImageLayersContainer.Size;
            PresentationLayerContainer.Size = ImageLayersContainer.Size;
            ControlLayer.Size = ImageLayersContainer.Size;

            // Инициира бутоните за управление на слоевете
            ToolButton.Pen = PenButton;
            ToolButton.Text = TextButton;
            ToolButton.Image = ImageButton;
            ToolButton.PenControl = PenControlPanel;
            ToolButton.TextControl = TextControlPanel;
            ToolButton.ImageControl = ImageControlPanel;
            ToolButton.Load();
            ToolButton.OnClick((Object)PenButton, EventArgs.Empty);

            // Инициира картинковия слой 
            ImageLayers.Initialize(16, ImageLayersContainer);
            ImageLayers.FullSize = new Size(FullSize.Width, FullSize.Height - BorderPanel.Height);
            ImageSelect.FrontButton = button5;
            ImageSelect.BackButton = button6;
            ImageSelect.DeleteButton = button7;

            // Инициира текстовия слой      
            TextLayers.Initialize(128, TextLayerContainer, ControlLayer);        
            TextLayerContainer.Image = ImageTools.EmptyBitmap(Color.Transparent, new Size(FullSize.Width, FullSize.Height - BorderPanel.Height));
            TextLayers.FullSize = new Size(FullSize.Width, FullSize.Height - BorderPanel.Height);

            // Инициира текстовите бутони
            TextButtons.Bold = BoldButton;
            TextButtons.Italic = ItalicButton;
            TextButtons.UnderLine = UnderlineButton;
            TextButtons.Strike = StrikeoutButton;
            TextButtons.Subscript = SubscriptButton;
            TextButtons.Superscript = SuperscriptButton;
            TextButtons.SizeBox = comboBox1;
            comboBox1.Text = "21";
            TextButtons.ColorBox = comboBox2;        
            TextButtons.Load();
            TextButtons.DisableAll();

            // Инициира писалковия слой
            PenTool.Initialize(PenLayerContainer, 1, Color.Black, new Size(FullSize.Width, FullSize.Height - BorderPanel.Height));
            comboBox3.Text = "1";

            // Инициира геометричните инструменти
            PresentationLayerContainer.Image = ImageTools.EmptyBitmap(Color.Transparent, new Size(FullSize.Width, FullSize.Height - BorderPanel.Height));
            PointSelection.Control = ControlLayer;
            GeometryTools.Container = PenLayerContainer;
            GeometryTools.PresentationContainer = PresentationLayerContainer;
            GeometryTools.MenuContainer = PenControlPanel;
            GeometryTools.LineButton = radioButton1;
            GeometryTools.AltitudeButton = radioButton2;
            GeometryTools.CircleButton = radioButton3;
            GeometryTools.MiddleButton = radioButton4;
            GeometryTools.CCircleButton = radioButton5;
            GeometryTools.RectangleButton = radioButton6;
            GeometryTools.Load();

            // Задава правоъгълниците за разширяване/свиване на формата
            RTop = new Rectangle(0, 0, this.ClientSize.Width, borderPadding);
            RLeft = new Rectangle(0, 0, borderPadding, this.ClientSize.Height);
            RBottom = new Rectangle(0, this.ClientSize.Height - borderPadding, this.ClientSize.Width, borderPadding);
            RRight = new Rectangle(this.ClientSize.Width - borderPadding, 0, borderPadding, this.ClientSize.Height);
            RTopLeft = new Rectangle(0, 0, borderPadding, borderPadding);
            RTopRight = new Rectangle(this.ClientSize.Width - borderPadding, 0, borderPadding, borderPadding);
            RBottomLeft = new Rectangle(0, this.ClientSize.Height - borderPadding, borderPadding, borderPadding);
            RBottomRight = new Rectangle(this.ClientSize.Width - borderPadding, this.ClientSize.Height - borderPadding, borderPadding, borderPadding);
        }

        // Обработва натискане на мишката
        private void ControlLayer_MouseDown(object sender, MouseEventArgs e)
        {   
            // Проверява дали настоящият инструмент е картинковият
            if (Tool.Current == Tools.Images)
            {
                // Определя индекса на картинката, която се намира на координатите;
                int l = ImageLayers.LayerOnCoordinates(e.Location);

                // Ако има селектирата картина и натискането е извън която и да е друга, селектирането се деактивира
                if (ImageSelect.Active && ImageSelect.Layer != l) ImageSelect.Deactivate();

                // Ако няма селектирана картинка инциира drag и задава cursor clip, за да не се измести картината извън пределите на видимата форма
                if (l != -1 && !ImageDrag.Active && (!ImageSelect.Active || ImageSelect.IsPointInImage(new Point(e.X, e.Y))))
                {
                    Point PanelLoc = this.PointToScreen(ImageLayersContainer.Location);
                    Point RectPointOne = new Point(PanelLoc.X + e.X - ImageLayers.Pictures[l].Location.X, PanelLoc.Y + e.Y - ImageLayers.Pictures[l].Location.Y);
                    Point RectPointTwo = new Point(PanelLoc.X + ImageLayersContainer.Width - (ImageLayers.Pictures[l].Width - (e.X - ImageLayers.Pictures[l].Location.X) - 2),
                                                   PanelLoc.Y + ImageLayersContainer.Height - (ImageLayers.Pictures[l].Height - (e.Y - ImageLayers.Pictures[l].Location.Y)) + 2);
                    Rectangle ClipRect = new Rectangle();
                    if (!ImageSelect.Active) ClipRect = new Rectangle(RectPointOne, new Size(RectPointTwo.X - RectPointOne.X, RectPointTwo.Y - RectPointOne.Y));
                    else ClipRect = new Rectangle(new Point(RectPointOne.X - ImageSelect.Margin - ImageSelect.SquareApothem, RectPointOne.Y - ImageSelect.Margin - ImageSelect.SquareApothem),
                                                  new Size(RectPointTwo.X - RectPointOne.X + 2 * ImageSelect.Margin + 2 * ImageSelect.SquareApothem, RectPointTwo.Y - RectPointOne.Y + 2 * ImageSelect.Margin + 2 * ImageSelect.SquareApothem));
                    Cursor.Clip = ClipRect;
                    ImageDrag.Activate(l, e.Location);
                }

                // Ако мишката е задържана върху квадрат за resize на картинка започва resize
                if (!ImageDrag.Active && ImageSelect.Active && !ImageSelect.ActiveResize && ImageSelect.CurrentSquareIndex != -1)
                {
                    Point ContLoc = this.PointToScreen(ImageLayersContainer.Location);
                    LocationAndSize las = new LocationAndSize();
                    las.Location = ContLoc;
                    las.Size = ImageLayersContainer.Size;
                    Rectangle Clip = ImageSelect.GenerateClip(las);
                    Cursor.Clip = Clip;
                    ImageSelect.ActiveResize = true;
                }
            }

            // Проверява дали настоящият инструмент е текстовият
            if (Tool.Current == Tools.Text)
            {
                // Ако има избрано текстово поле и курсорът е в drag зоната, започва drag
                if (TextSelect.Active && TextSelect.IsInDragZone(e.Location)) TextDrag.Activate(TextSelect.Layer, e.Location);

                // Ако курсорът е върху квадрат за resize, започва Resize
                if (TextSelect.Active && !TextSelect.ActiveResize && TextSelect.CurrentSquareIndex != -1)
                {
                    Point ContLoc = this.PointToScreen(TextLayerContainer.Location);
                    LocationAndSize las = new LocationAndSize();
                    las.Location = ContLoc;
                    las.Size = TextLayerContainer.Size;
                    Rectangle Clip = TextSelect.GenerateClip(las);
                    Cursor.Clip = Clip;
                    TextSelect.ActiveResize = true;
                }
            }

            // Проверява дали настоящият инструмент е маркерът; ако е, започва чертане
            if (Tool.Current == Tools.Pen && !PenTool.Active) PenTool.StartDrawing(e.Location);
        }

        // Обработва местене на мишката
        private void ControlLayer_MouseMove(object sender, MouseEventArgs e)
        {
            // Проверява дали настоящият инструмент е картинковият
            if (Tool.Current == Tools.Images)
            {
                // Ако има активен Image Drag, актуализира положението на картинката
                if (ImageDrag.Active) ImageDrag.Update(e.Location);
                // Ако няма активен resize, но има селектирана картинка, но няма активен resize, следи за вида на курсора
                if (!ImageDrag.Active && ImageSelect.Active && !ImageSelect.ActiveResize)
                {
                    int SquareIndex = ImageSelect.SquareIndex(e.X, e.Y);
                    ImageSelect.CurrentSquareIndex = SquareIndex;
                    if (SquareIndex != -1)
                    {
                        Cursor.Current = ImageSelect.CursorSquares[SquareIndex].Cursor;
                    }
                    else Cursor.Current = Cursors.Default;
                }
                // Ако има активен resize, променя позицията/размера на картинката
                if (!ImageDrag.Active && ImageSelect.Active && ImageSelect.ActiveResize) ImageSelect.Resize(new Point(e.X, e.Y));
            }
            // Проверява дали настоящият инструмент е текстовия
            if (Tool.Current == Tools.Text)
            {
                // Ако има избрано текстово поле, но няма активен resize следи за вида на курсора (вид стрелка или drag зона)
                if (TextSelect.Active && !TextSelect.ActiveResize)
                {
                    int SquareIndex = TextSelect.SquareIndex(e.Location);
                    TextSelect.CurrentSquareIndex = SquareIndex;
                    if (TextSelect.IsInDragZone(e.Location))
                    {
                        Cursor.Current = Cursors.SizeAll;
                    }
                    else if (SquareIndex != -1)
                    {
                        Cursor.Current = TextSelect.CursorSquares[SquareIndex].Cursor;
                    }
                    else Cursor.Current = Cursors.Default;
                }
                // Ако има активен текстов drag, актуализира позицията на полето
                if (TextDrag.Active) TextDrag.Update(e.Location);
                // Ако има активен текстов resize, актуализира позицията/размера на полето
                if (TextSelect.ActiveResize) TextSelect.Resize(e.Location);
            }
            // Проверява дали настоящият инструмент е маркера и дали се чертае; чертае линия, ако е така
            if (Tool.Current == Tools.Pen && PenTool.Active) PenTool.DrawMove(e.Location);
        }
        // Обработва пускане на мишката
        private void ControlLayer_MouseUp(object sender, MouseEventArgs e)
        {
            // Проверява дали настоящият инструмент е картинковият
            if (Tool.Current == Tools.Images)
            {
                // Ако има активен картинков drag, го прекратява и премахва ограничението на курсова
                if (ImageDrag.Active)
                {
                    ImageDrag.Deactivate();
                    Cursor.Clip = new Rectangle();
                }
                // Ако има активен картинков resize, го прекратява и премахва ограничението на курсова 
                if (!ImageDrag.Active && ImageSelect.Active && ImageSelect.ActiveResize)
                {
                    Cursor.Clip = new Rectangle();
                    ImageSelect.StopResize();
                }
            }
            // Проверява дали настоящият инструмент е текстовия
            if (Tool.Current == Tools.Text)
            {
                // Ако има активен текстов resize, го прекратява
                if (TextSelect.ActiveResize)
                {
                    TextSelect.ActiveResize = false;
                    TextSelect.SetCursorSquares();
                    TextSelect.CurrentSquareIndex = -1;
                    Cursor.Clip = new Rectangle();
                }
                // Ако има активен текстов drag, го прекратява
                if (TextDrag.Active) TextDrag.Deactivate();
            }
            // Проверява дали настоящият инструмент е маркерът; ако има активно чертаене, го прекратява
            if (Tool.Current == Tools.Pen && PenTool.Active) PenTool.StopDrawing();
        }
        // Обработка кликане на мишката
        private void ControlLayer_MouseClick(object sender, MouseEventArgs e)
        {
            // Проверяна дали настоящият инструмент е текстовият
            if (Tool.Current == Tools.Text)
            {
                // Определя индекса на текстовото поле, върху което е курсорът; ако няма такова, l приема стойност -1
                int l = TextLayers.LayerOnCoordinates(e.Location);

                // Ако няма избрано текстово поле и кликването е извън кое да е поле, се създава ново поле
                if (!TextSelect.Active && l == -1)
                {
                    int fontSize;
                    if (!int.TryParse(comboBox1.Text, out fontSize)) fontSize = 21;
                    TextLayers.AddLayer(e.Location, TextLayers.DefaultSize, fontSize);
                }
                // Ако няма избрано текстово поле, но курсорът е върху такова, то се селектира
                else if (!TextSelect.Active && l != -1)
                {
                    TextSelect.Activate(l);
                }
                // Ако има избрано поле, но кликването е извън кое да е такова, селектирането се прикратява
                else if (TextSelect.Active && l == -1)
                {
                    TextSelect.Deactivate();
                }
                // Ако има избрано поле, а кликването е върху друго такова, настоящото селектиране се прекратява и се
                // прехвърля върху другото поле
                else if (TextSelect.Active && l != -1 && l != TextSelect.Layer)
                {
                    TextSelect.Deactivate();
                    TextSelect.Activate(l);
                }
            }

            // Проверява дали настоящият инструмент е картинковият
            if (Tool.Current == Tools.Images)
            {
                // Определя индекса на картинката, върху която е курсорът; ако няма такава, l приема стойност -1
                int l = ImageLayers.LayerOnCoordinates(e.Location);
                // Ако курсорът е върху някоя картинка и няма вече избрана такава, тя се селектира
                if (l != -1 && !ImageSelect.Active) ImageSelect.Activate(l);
                // Ако има селектирана картинка и кликването е извън нея, селектирането се прекратява
                if (ImageSelect.Active && l != ImageSelect.Layer) ImageSelect.Deactivate();
            }
        }
        // Обработва натискане на бутона за добавяне на картинка
        private void button4_Click(object sender, EventArgs e)
        {
            // Ако има избрана картинка, селектирането се прекратява
            if (ImageSelect.Active) ImageSelect.Deactivate();
            // Избиране на картинката от файл
            OpenFileDialog f = new OpenFileDialog();
            f.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.gif) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.gif";
            f.Multiselect = false;
            if (f.ShowDialog() == DialogResult.OK && File.Exists(f.FileName))
            {
                Image Img = Image.FromFile(f.FileName);
                // Scale-ва картинката така, че размерите и да са поне два пъти по-малко от съответстващите размери на картинковото поле
                Size NewSize = ImageTools.ScaleToFit(Img.Size, ImageLayersContainer.Width / 2, ImageLayersContainer.Height / 2);
                // Пробва да добави картинката
                AddLayerData Response = ImageLayers.AddLayer((Bitmap)Img, new Point(0, MenuButton.Location.Y + MenuButton.Height), NewSize);
                // Извежда съобщения за невалиден размер и достигнато ограничение на картинките
                if (Response == AddLayerData.InvalidSize) MessageBox.Show("Image has invalid size!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Response == AddLayerData.MaxLayersReached) MessageBox.Show("You have reached the maximum number of images!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Обработва натискане на бутона за изтриване на селектирана картинка
        private void button7_Click(object sender, EventArgs e)
        {
            ImageLayers.DeleteLayer(ImageSelect.Layer);
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            ControlLayer.Focus();
        }
        // Следи за натискане на специални клавиши в приложението
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Дефинира възможни комбинации за добавяне на символ с Alt+Key
            KeyCombination[] kc = new KeyCombination[]
                {
                    new KeyCombination(Keys.A, 'α'),
                    new KeyCombination(Keys.B, 'β'),
                    new KeyCombination(Keys.C, 'γ'),
                    new KeyCombination(Keys.D, 'Δ'),
                    new KeyCombination(Keys.I, '∞'),
                    new KeyCombination(Keys.P, 'π'),
                    new KeyCombination(Keys.F, 'φ'),
                    new KeyCombination(Keys.R, 'ρ'),
                    new KeyCombination(Keys.W, 'ω'),
                    new KeyCombination(Keys.Right, '→'),
                    new KeyCombination(Keys.Left, '←'),
                    new KeyCombination(Keys.Up, '↑'),
                    new KeyCombination(Keys.Down, '↓'),
                    new KeyCombination(Keys.V, '∢'),
                    new KeyCombination(Keys.M, '≡')
                };
            // Ако има селектирано текстово поле, проверява дали не е избрана комбинация за добавяне на специален символ
            if (TextSelect.Active)
            {
                for (int i = 0; i < kc.Length; i++)
                {
                    if (e.KeyCode == kc[i].Key && e.Modifiers == Keys.Alt)
                    {
                        RichTextBox rtb = TextLayers.Layers[TextSelect.Layer].TextBox;
                        rtb.Select(rtb.TextLength, 1);
                        rtb.SelectedText = kc[i].Symbol.ToString();
                        rtb.SelectionStart = rtb.TextLength;
                        rtb.SelectionLength = 0;
                    }
                }
            }
            // Изтрива селектирана картинка с Delete
            if (Tool.Current == Tools.Images && ImageSelect.Active && e.KeyCode == Keys.Delete)
            {
                ImageLayers.DeleteLayer(ImageSelect.Layer);
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
            }
            // Изтрива селектирано текстово поле с Delete, ако няма селектиран текст
            if (Tool.Current == Tools.Text && TextSelect.Active &&
                TextLayers.Layers[TextSelect.Layer].TextBox.SelectionLength == 0 && e.KeyCode == Keys.Delete)
            {
                TextLayers.DeleteLayer(TextSelect.Layer);
            }
            // Комбинация Ctrl+V за добавяна на картинка от clipboard-а
            if (!ImageSelect.Active && e.KeyCode == Keys.V && e.Modifiers == Keys.Control && Clipboard.ContainsImage())
            {
                ImageButton.PerformClick();
                Image Img = Clipboard.GetImage();
                Size NewSize = ImageTools.ScaleToFit(Img.Size, ImageLayersContainer.Width / 2, ImageLayersContainer.Height / 2);
                AddLayerData Response = ImageLayers.AddLayer((Bitmap)Img, new Point(0, MenuButton.Location.Y + MenuButton.Height), NewSize);
                if (Response == AddLayerData.InvalidSize) MessageBox.Show("Image has invalid size!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Response == AddLayerData.MaxLayersReached) MessageBox.Show("You have reached the maximum number of images!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Комбинация Ctrl+Down за бързо прехвърляне в subscript
            if (TextSelect.Active && e.KeyCode == Keys.Down && e.Modifiers == Keys.Control)
            {
                TextSelect.SubscriptSelected();
            }
            // Комбинация Ctrl+Up за бързо прехвърляне в superscript
            if (TextSelect.Active && e.KeyCode == Keys.Up && e.Modifiers == Keys.Control)
            {
                TextSelect.SuperscriptSelected();
            }
        }
        // При натискане на бутона за избиране на текстовия инструмент, прекратява селектирането на картинка (ако има)
        private void TextButton_Click(object sender, EventArgs e)
        {
            if (ImageSelect.Active) ImageSelect.Deactivate();
        }
        // Включва гъбата за триене на маркер
        private void button8_Click(object sender, EventArgs e)
        {
            GeometryTools.DisableButtons();
            Tool.Current = Tools.Pen;
            Button b = sender as Button;

            if (PenTool.Sponge)
            {
                PenTool.Sponge = false;
                PenColor.Enabled = true;
                b.BackColor = ToolButton.InactiveColor;
                b.FlatAppearance.BorderSize = 0;
            }

            else
            {
                PenTool.Sponge = true;
                PenColor.Enabled = false;
                b.BackColor = Color.Gray;            
            }
        }
        // Променя дебелината на маркера
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeometryTools.DisableButtons();
            ComboBox cbo = sender as ComboBox;
            PenTool.PenSize = int.Parse(cbo.Text);
        }
        // Сменя цвета на маркера
        private void PenColor_Click(object sender, EventArgs e)
        {
            GeometryTools.DisableButtons();
            Tool.Current = Tools.Pen;
            ColorDialog c = new ColorDialog();

            if (c.ShowDialog() == DialogResult.OK)
            {
                (sender as Button).BackColor = c.Color;
                PenTool.PenColor = c.Color;
            }
        }
        // Изтрива писаното с маркер
        private void button9_Click(object sender, EventArgs e)
        {
            GeometryTools.DisableButtons();
            if (GeometryTools.ps != null) GeometryTools.ps.End();
            using (Graphics g = Graphics.FromImage(PresentationLayerContainer.Image)) g.Clear(Color.Transparent);
            PenTool.Clear();
        }
    }

    public class Tool
    {
        public static Tool Current = Tools.Pen;
    }
    public class Tools
    {
        public static Tool Pen = new Tool();
        public static Tool Text = new Tool();
        public static Tool Images = new Tool();
        public static Tool Geometry = new Tool();
    }
    class KeyCombination
    {
        public Keys Key;
        public char Symbol;

        public KeyCombination(Keys Key, char Symbol)
        {
            this.Key = Key;
            this.Symbol = Symbol;
        }
    }
    public class ToolButton
    {
        public static Panel PenControl, TextControl, ImageControl;

        public static Button Pen
        {
            get { return PPen; }
            set
            {
                PPen = value;
                PenLoc = new Point(value.Location.X, value.Location.Y);
                PenSize = new Size(value.Width, value.Height);
                value.Click += OnClick;
                value.Enter += OnFocus;
            }
        }

        public static Button Text
        {
            get { return PText; }
            set
            {
                PText = value;
                TextLoc = new Point(value.Location.X, value.Location.Y);
                TextSize = new Size(value.Width, value.Height);
                value.Click += OnClick;
                value.Enter += OnFocus;
            }
        }

        public static Button Image
        {
            get { return PImage; }
            set
            {
                PImage = value;
                ImageLoc = new Point(value.Location.X, value.Location.Y);
                ImageSize = new Size(value.Width, value.Height);
                value.Click += OnClick;
                value.Enter += OnFocus;
            }
        }

        private static Point PenLoc, TextLoc, ImageLoc;
        private static Size PenSize, TextSize, ImageSize;
        private static Button PPen, PText, PImage;
        public static Color InactiveColor = Color.Gainsboro, ActiveColor = Color.Silver, ActiveBorderColor = Color.Gray;

        public static void Load()
        {
            TextControl.Location = PenControl.Location;
            ImageControl.Location = PenControl.Location;
            TextControl.Size = PenControl.Size;
            ImageControl.Size = PenControl.Size;
        }

        public static void OnClick(object sender, EventArgs e)
        {
            GeometryTools.DisableButtons();
            Button b = sender as Button;
            if (TextSelect.Active) TextSelect.Deactivate();
            if (ImageSelect.Active) ImageSelect.Deactivate();
            Pen.BackColor = InactiveColor;
            Pen.FlatAppearance.BorderSize = 0;
            Pen.FlatAppearance.MouseDownBackColor = InactiveColor;
            Pen.FlatAppearance.MouseOverBackColor = InactiveColor;
            Pen.Location = PenLoc;
            Pen.Size = PenSize;
            Text.BackColor = InactiveColor;
            Text.FlatAppearance.BorderSize = 0;
            Text.FlatAppearance.MouseDownBackColor = InactiveColor;
            Text.FlatAppearance.MouseOverBackColor = InactiveColor;
            Text.Location = TextLoc;
            Text.Size = TextSize;
            Image.BackColor = InactiveColor;
            Image.FlatAppearance.BorderSize = 0;
            Image.FlatAppearance.MouseDownBackColor = InactiveColor;
            Image.FlatAppearance.MouseOverBackColor = InactiveColor;
            Image.Location = ImageLoc;
            Image.Size = ImageSize;
            b.BackColor = ActiveColor;
            b.FlatAppearance.BorderColor = ActiveBorderColor;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor = ActiveColor;
            b.FlatAppearance.MouseDownBackColor = ActiveColor;
            b.Location = new Point(b.Location.X - 1, b.Location.Y - 1);
            b.Size = new Size(b.Width + 2, b.Height + 2);
            PenControl.Hide();
            TextControl.Hide();
            ImageControl.Hide();
            if (b == Pen) Tool.Current = Tools.Pen;
            if (b == Pen) PenControl.Show();
            if (b == Text) Tool.Current = Tools.Text;
            if (b == Text) TextControl.Show();
            if (b == Image) Tool.Current = Tools.Images;
            if (b == Image) ImageControl.Show();
        }

        public static void OnFocus(object sender, EventArgs e)
        {
            Button b = sender as Button;
            b.Parent.Focus();
        }
    }
    public class MenuAnimation
    {
        public static int time = 100, fps = 50, height = 85;
        public static Panel Panel;
        private static int period, move, frames, finishedFrames;
        public static bool Visible = true, Active = false;

        public static void Animate()
        {
            if (!Active)
            {
                Active = true;
                finishedFrames = 0;
                period = 1000 / fps;
                frames = time / period;
                move = height / frames;

                while (Active)
                {
                    DoMove();
                    Thread.Sleep(period);
                }
            }
        }

        private static void DoMove()
        {
            if (!Visible)
            {
                if (finishedFrames < frames)
                {
                    Panel.Height += move;
                    finishedFrames++;
                }
                else
                {
                    Panel.Height = height;
                    Active = false;
                    Visible = true;
                }
            }

            else
            {
                if (finishedFrames < frames)
                {
                    Panel.Height -= move;
                    finishedFrames++;
                }
                else
                {
                    Panel.Height = 0;
                    Active = false;
                    Visible = false;
                }
            }

            Panel.Parent.Refresh();
        }
    }
    public class LocationAndSize
    {
        public Point Location;
        public Size Size;

        public Rectangle ToRectangle()
        {
            return new Rectangle(this.Location, this.Size);
        }

        public static LocationAndSize FromPoints(Point A, Point B)
        {
            LocationAndSize output = new LocationAndSize();
            output.Location = new Point(Math.Min(A.X, B.X), Math.Min(A.Y, B.Y));
            output.Size = new Size(Math.Max(A.X, B.X) - output.Location.X + 1, Math.Max(A.Y, B.Y) - output.Location.Y + 1);
            return output;
        }
    }
    public class CursorSquare
    {
        private int StartX, EndX, StartY, EndY;
        public Cursor Cursor;
        public Rectangle CursorClip;

        public CursorSquare(Point Center, int Apothem, Cursor Cursor)
        {
            this.StartX = Center.X - Apothem;
            this.EndX = Center.X + Apothem;
            this.StartY = Center.Y - Apothem;
            this.EndY = Center.Y + Apothem;
            this.Cursor = Cursor;
        }

        public bool IsPointInside(Point Point)
        {
            return (Point.X >= this.StartX && Point.X <= this.EndX && Point.Y >= this.StartY && Point.Y <= this.EndY);
        }
    }
    public class AddLayerData
    {
        public static AddLayerData OK = new AddLayerData();
        public static AddLayerData InvalidSize = new AddLayerData();
        public static AddLayerData MaxLayersReached = new AddLayerData();
    }
    public class ImageTools
    {
        public static Bitmap MakeTransparent(Image image)
        {
            Bitmap b = new Bitmap(image);

            var replacementColour = Color.FromArgb(255, 255, 255);
            var tolerance = 200;

            for (int i = b.Size.Width - 1; i >= 0; i--)
            {
                for (int j = b.Size.Height - 1; j >= 0; j--)
                {
                    var col = b.GetPixel(i, j);

                    if (255 - col.R < tolerance &&
                        255 - col.G < tolerance &&
                        255 - col.B < tolerance)
                    {
                        b.SetPixel(i, j, replacementColour);
                    }
                }
            }

            b.MakeTransparent(replacementColour);

            return b;
        }

        public static Size ScaleToFit(Size img, int MaxWidth, int MaxHeight)
        {
            double ratio = (double)img.Width / (double)img.Height;
            int nw = img.Width, nh = img.Height;
            if (nw > MaxWidth)
            {
                nw = MaxWidth;
                nh = (int)Math.Ceiling((decimal)nw / (decimal)ratio);
            }

            if (nh > MaxHeight)
            {
                nh = MaxHeight;
                nw = (int)Math.Ceiling((decimal)nh * (decimal)ratio);
            }
            return new Size(nw, nh);
        }

        public static Bitmap EmptyBitmap(Color Background, Size Size)
        {
            Bitmap output = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(output)) g.Clear(Background);
            return output;
        }
    }
    public class ImageLayer
    {
        public Point Location;
        public Size Size;

        public void Clear()
        {
            this.Image.Dispose();
            this.OImage.Dispose();
        }

        public ImageLayer Copy()
        {
            Bitmap OutputImage = new Bitmap(Image, Size);
            return new ImageLayer(new Point(Location.X, Location.Y), new Size(Size.Width, Size.Height), OutputImage);
        }

        public int Width
        {
            get
            {
                return this.Size.Width;
            }
            set
            {
                this.Size.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.Size.Height;
            }
            set
            {
                this.Size.Height = value;
            }
        }
        public Bitmap Image;
        private Bitmap OImage;
        public Bitmap OriginalImage
        {
            get
            {
                return OImage;
            }
        }

        public ImageLayer(Point Location, Size Size, Bitmap Image)
        {
            this.Location = Location;
            this.Size = Size;
            this.Image = new Bitmap(Image, Size);
            this.OImage = new Bitmap(Image, ImageTools.ScaleToFit(Image.Size, 1200, 1200));
        }
    }
    public class ImageLayers
    {
        public static int MinSize = 48;

        public static ImageLayer[] Pictures;
        public static int CurrentNumberOfLayers = 0;
        public static PictureBox ImageContainer;
        public static Size FullSize;

        public static void Initialize(int MaxLayers, PictureBox Container)
        {
            Pictures = new ImageLayer[MaxLayers];
            ImageContainer = Container;
        }

        public static void Refresh()
        {
            ImageContainer.Refresh();
        }

        public static AddLayerData AddLayer(Bitmap Image, Point Location, Size Size)
        {
            if (Size.Width < MinSize || Size.Height < MinSize || Size.Width > ImageContainer.Width || Size.Height > ImageContainer.Height) return AddLayerData.InvalidSize;
            else if (CurrentNumberOfLayers < Pictures.Length)
            {
                Pictures[CurrentNumberOfLayers] = new ImageLayer(Location, Size, Image);
                CurrentNumberOfLayers++;
                Reload();
                return AddLayerData.OK;
            }
            else return AddLayerData.MaxLayersReached;
        }

        public static bool IsBack(int layer)
        {
            return (layer == 0);
        }

        public static bool IsFront(int layer)
        {
            return (layer == CurrentNumberOfLayers - 1);
        }

        public static void MoveLayerFront(int Layer)
        {
            if (!IsFront(Layer))
            {
                ImageLayer ObjectHolder = Pictures[Layer + 1];
                Pictures[Layer + 1] = Pictures[Layer];
                Pictures[Layer] = ObjectHolder;
                ImageSelect.Deactivate();
                ImageSelect.Activate(Layer + 1);
                Reload();
            }
        }

        public static void MoveLayerBack(int Layer)
        {
            if (!IsBack(Layer))
            {
                ImageLayer ObjectHolder = Pictures[Layer - 1];
                Pictures[Layer - 1] = Pictures[Layer];
                Pictures[Layer] = ObjectHolder;
                ImageSelect.Deactivate();
                ImageSelect.Activate(Layer - 1);
                Reload();
            }
        }

        public static void DeleteLayer(int Layer)
        {

            if (Layer < CurrentNumberOfLayers)
            {
                Pictures[Layer].Clear();
                CurrentNumberOfLayers--;
                for (int i = Layer; i < CurrentNumberOfLayers; i++)
                {
                    Pictures[i] = Pictures[i + 1].Copy();
                    Pictures[i + 1].Clear();
                }
                Pictures[CurrentNumberOfLayers].Clear();
                ImageSelect.DeleteDeactivate();
                Reload();
            }

        }

        public static int LayerOnCoordinates(Point Loc)
        {
            int output = -1;
            for (int i = CurrentNumberOfLayers - 1; i >= 0; i--)
            {
                ImageLayer pb = Pictures[i];
                Point Location = pb.Location;
                if (Loc.X >= Location.X && Loc.X < Location.X + pb.Width && Loc.Y >= Location.Y && Loc.Y < Location.Y + pb.Height)
                {
                    output = i;
                    break;
                }
            }
            return output;
        }

        public static void Reload()
        {
            if (ImageContainer.Image != null) ImageContainer.Image.Dispose();
            ImageContainer.Image = DrawLayers();
        }

        public static Bitmap DrawLayers()
        {
            Bitmap output = new Bitmap(FullSize.Width, FullSize.Height, PixelFormat.Format16bppRgb555);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.Clear(Color.White);
                for (int i = 0; i < CurrentNumberOfLayers; i++) g.DrawImage(Pictures[i].Image, new Rectangle(Pictures[i].Location, Pictures[i].Size));

            }
            return output;
        }
    }
    public class ImageDrag
    {
        public static bool Active = false;
        public static int initX, initY, initLocX, initLocY, Layer;

        public static void Activate(int Layer, Point Location)
        {
            ImageDrag.Active = true;
            ImageDrag.Layer = Layer;
            ImageDrag.initX = Location.X;
            ImageDrag.initY = Location.Y;
            ImageDrag.initLocX = ImageLayers.Pictures[Layer].Location.X;
            ImageDrag.initLocY = ImageLayers.Pictures[Layer].Location.Y;
        }

        public static void Update(Point Location)
        {
            ImageLayers.Pictures[Layer].Location = new Point(initLocX + Location.X - initX, initLocY + Location.Y - initY);
            ImageLayers.Reload();
        }

        public static void Deactivate()
        {
            Active = false;
        }
    }
    public class ImageSelect
    {
        public static int Margin = 10, SquareApothem = 3;

        public static bool Active = false;
        public static bool ActiveResize = false;
        public static PictureBox Container;
        public static ImageLayer Image;
        public static Bitmap OriginalImage;
        public static int Layer;
        public static Size Size;
        public static CursorSquare[] CursorSquares = new CursorSquare[8];
        public static int CurrentSquareIndex = -1;
        public static Button FrontButton, BackButton, DeleteButton;

        public static Rectangle GenerateClip(LocationAndSize Container)
        {
            int MinSize = ImageLayers.MinSize;
            LocationAndSize RectData = new LocationAndSize();
            Point Loc = Container.Location;
            Size ContSize = Container.Size;
            switch (CurrentSquareIndex)
            {
                case 0:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y - Margin - SquareApothem), new Point(Loc.X + Image.Location.X + Image.Width - MinSize, Loc.Y + Image.Location.Y + Image.Height - MinSize));
                    break;
                case 1:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + Image.Location.Y + Image.Height - MinSize), new Point(Loc.X + ContSize.Width + 2 * Margin + 2 * SquareApothem, Loc.Y - Margin - SquareApothem));
                    break;
                case 2:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Image.Location.X + MinSize, Loc.Y + Image.Location.Y + Image.Height - MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y - Margin - SquareApothem));
                    break;
                case 3:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Image.Location.X + MinSize, Loc.Y - Margin - SquareApothem), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 4:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Image.Location.X + MinSize, Loc.Y + Image.Location.Y + MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 5:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + Image.Location.Y + MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 6:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + ContSize.Width + Margin + SquareApothem), new Point(Loc.X + Image.Location.X + Image.Width - MinSize, Loc.Y + Image.Location.Y + MinSize));
                    break;
                case 7:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y - Margin - SquareApothem), new Point(Loc.X + Image.Location.X + Image.Width - MinSize, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
            }
            return RectData.ToRectangle();
        }

        public static bool IsPointInImage(Point Point)
        {
            return (Point.X >= Image.Location.X + Margin + SquareApothem && Point.X < Image.Location.X + Image.Width - Margin - SquareApothem &&
                    Point.Y >= Image.Location.Y + Margin + SquareApothem && Point.Y < Image.Location.Y + Image.Height - Margin - SquareApothem);
        }

        public static void Activate(int Layer)
        {
            ImageSelect.Layer = Layer;
            ImageSelect.Image = ImageLayers.Pictures[Layer];
            ImageSelect.OriginalImage = new Bitmap(Image.Image, Image.Size);
            ImageSelect.Size = Image.Size;
            DrawBitmap(false);
            ImageSelect.Active = true;
            FrontButton.Enabled = !ImageLayers.IsFront(Layer);
            BackButton.Enabled = !ImageLayers.IsBack(Layer);
            DeleteButton.Enabled = true;
        }

        public static void Deactivate()
        {
            Active = false;
            ActiveResize = false;
            Image.Location = new Point(Image.Location.X + Margin + SquareApothem, Image.Location.Y + Margin + SquareApothem);
            Image.Size = new Size(Image.Width - 2 * Margin - 2 * SquareApothem, Image.Height - 2 * Margin - 2 * SquareApothem);
            Bitmap FinalImage = new Bitmap(Image.OriginalImage, Image.Size);
            Image.Image = FinalImage;
            FrontButton.Enabled = false;
            BackButton.Enabled = false;
            DeleteButton.Enabled = false;
            OriginalImage.Dispose();
            ImageLayers.Reload();
        }

        public static void DeleteDeactivate()
        {
            Active = false;
            ActiveResize = false;
            OriginalImage.Dispose();
        }

        public static void StopResize()
        {
            ActiveResize = false;
            DrawBitmap(true);
        }

        public static int SquareIndex(int X, int Y)
        {
            Point InImageLocation = new Point(X - Image.Location.X, Y - Image.Location.Y);
            int output = 0;
            while (output != -1)
            {
                if (output == CursorSquares.Length) output = -1;
                else if (CursorSquares[output].IsPointInside(InImageLocation)) break;
                else output++;
            }
            return output;
        }

        public static void Resize(Point MousePoint)
        {
            Cursor.Current = CursorSquares[CurrentSquareIndex].Cursor;
            LocationAndSize ImagePosition = new LocationAndSize();
            Point CurrentAPoint = Image.Location;
            Point CurrentBPoint = new Point(CurrentAPoint.X + Image.Width - 1, CurrentAPoint.Y + Image.Height - 1);

            switch (CurrentSquareIndex)
            {
                case 0:
                    ImagePosition = LocationAndSize.FromPoints(MousePoint, CurrentBPoint);
                    break;
                case 1:
                    ImagePosition = LocationAndSize.FromPoints(new Point(CurrentAPoint.X, MousePoint.Y), CurrentBPoint);
                    break;
                case 2:
                    ImagePosition = LocationAndSize.FromPoints(new Point(CurrentAPoint.X, CurrentBPoint.Y), MousePoint);
                    break;
                case 3:
                    ImagePosition = LocationAndSize.FromPoints(new Point(MousePoint.X, CurrentBPoint.Y), CurrentAPoint);
                    break;
                case 4:
                    ImagePosition = LocationAndSize.FromPoints(CurrentAPoint, MousePoint);
                    break;
                case 5:
                    ImagePosition = LocationAndSize.FromPoints(new Point(CurrentBPoint.X, MousePoint.Y), CurrentAPoint);
                    break;
                case 6:
                    ImagePosition = LocationAndSize.FromPoints(new Point(CurrentBPoint.X, CurrentAPoint.Y), MousePoint);
                    break;
                case 7:
                    ImagePosition = LocationAndSize.FromPoints(new Point(MousePoint.X, CurrentAPoint.Y), CurrentBPoint);
                    break;
                default:
                    ImagePosition = LocationAndSize.FromPoints(CurrentAPoint, CurrentBPoint);
                    break;
            }

            Image.Location = ImagePosition.Location;
            Image.Size = ImagePosition.Size;
            DrawBitmap(false);
        }

        public static void DrawBitmap(bool UseOriginal)
        {
            Bitmap output;

            if (!Active)
            {
                Image.Size = new Size(Image.Width + 2 * Margin + 2 * SquareApothem, Image.Height + 2 * Margin + 2 * SquareApothem);
                Image.Location = new Point(Image.Location.X - Margin - SquareApothem, Image.Location.Y - Margin - SquareApothem);
            }

            output = new Bitmap(Image.Width, Image.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(output))
            {
                g.Clear(Color.Transparent);
                SolidBrush sb = new SolidBrush(Color.Black);
                SolidBrush gray = new SolidBrush(Color.Gray);
                if (UseOriginal) g.DrawImage(Image.OriginalImage, new Rectangle(new Point(SquareApothem + Margin, SquareApothem + Margin), new Size(Image.Width - 2 * Margin - 2 * SquareApothem, Image.Height - 2 * Margin - 2 * SquareApothem)));
                else if (!ActiveResize) g.DrawImage(OriginalImage, new Rectangle(new Point(SquareApothem + Margin, SquareApothem + Margin), new Size(Image.Width - 2 * Margin - 2 * SquareApothem, Image.Height - 2 * Margin - 2 * SquareApothem)));
                else g.FillRectangle(gray, new Rectangle(new Point(SquareApothem + Margin, SquareApothem + Margin), new Size(Image.Width - 2 * Margin - 2 * SquareApothem, Image.Height - 2 * Margin - 2 * SquareApothem)));
                Rectangle square = new Rectangle(new Point(0, 0), new Size(SquareApothem * 2 + 1, SquareApothem * 2 + 1));
                g.FillRectangle(sb, square);
                square.Location = new Point(Image.Width - 2 * SquareApothem - 1, 0);
                g.FillRectangle(sb, square);
                square.Location = new Point(Image.Width / 2 - SquareApothem, 0);
                g.FillRectangle(sb, square);
                square.Location = new Point(0, Image.Height - 2 * SquareApothem - 1);
                g.FillRectangle(sb, square);
                square.Location = new Point(Image.Width - 2 * SquareApothem - 1, Image.Height - 2 * SquareApothem - 1);
                g.FillRectangle(sb, square);
                square.Location = new Point(Image.Width / 2 - SquareApothem, Image.Height - 2 * SquareApothem - 1);
                g.FillRectangle(sb, square);
                square.Location = new Point(0, Image.Height / 2 - SquareApothem);
                g.FillRectangle(sb, square);
                square.Location = new Point(Image.Width - 2 * SquareApothem - 1, Image.Height / 2 - SquareApothem);
                g.FillRectangle(sb, square);
                CursorSquares[0] = new CursorSquare(new Point(SquareApothem, SquareApothem), SquareApothem, Cursors.SizeNWSE);
                CursorSquares[1] = new CursorSquare(new Point(Image.Width / 2, SquareApothem), SquareApothem, Cursors.SizeNS);
                CursorSquares[2] = new CursorSquare(new Point(Image.Width - SquareApothem, SquareApothem), SquareApothem, Cursors.SizeNESW);
                CursorSquares[3] = new CursorSquare(new Point(Image.Width - SquareApothem, Image.Height / 2), SquareApothem, Cursors.SizeWE);
                CursorSquares[4] = new CursorSquare(new Point(Image.Width - SquareApothem, Image.Height - SquareApothem), SquareApothem, Cursors.SizeNWSE);
                CursorSquares[5] = new CursorSquare(new Point(Image.Width / 2, Image.Height - SquareApothem), SquareApothem, Cursors.SizeNS);
                CursorSquares[6] = new CursorSquare(new Point(SquareApothem, Image.Height - SquareApothem), SquareApothem, Cursors.SizeNESW);
                CursorSquares[7] = new CursorSquare(new Point(SquareApothem, Image.Height / 2), SquareApothem, Cursors.SizeWE);
                Image.Image.Dispose();
                Image.Image = output;
                ImageLayers.Reload();
            }
        }
    }
    public class TextLayer
    {
        public static int Padding = 10, Margin = 3, SquareApothem = Margin;

        private Point PLocation;
        private Size PSize;
        public float OriginalFontSize;
        public Point Location
        {
            get { return PLocation; }

            set
            {
                PLocation = value;
                TextBox.Location = new Point(value.X + Margin + Padding, value.Y + Margin + Padding);
            }
        }
        public Size Size
        {
            get { return PSize; }
            set
            {
                PSize = value;
                TextBox.Size = new Size(value.Width - 2 * Margin - 2 * Padding, value.Height - 2 * Margin - 2 * Padding);
            }
        }
        public RichTextBox TextBox;
        public bool Selected = false;

        public Bitmap Image
        {
            get
            {
                if (Selected) return EditImage;
                else return DisplayImage;
            }
        }
        public void Clear()
        {
            this.DisplayImage.Dispose();
            this.EditImage.Dispose();
            this.TextBox.Enabled = false;
            this.TextBox.Visible = false;
        }

        public int Width
        {
            get
            {
                return this.Size.Width;
            }
            set
            {
                this.PSize.Width = value;
            }
        }
        public int Height
        {
            get
            {
                return this.Size.Height;
            }
            set
            {
                this.PSize.Height = value;
            }
        }
        public Bitmap DisplayImage, EditImage;

        public TextLayer(Point Location, RichTextBox TextBox)
        {
            this.PLocation = new Point(Location.X - Margin - Padding, Location.Y - Margin - Padding);
            this.PSize = new Size(TextBox.Width + 2 * Margin + 2 * Padding, TextBox.Height + 2 * Margin + 2 * Padding);
            this.TextBox = TextBox;
            this.OriginalFontSize = TextBox.Font.Size;
            this.Refresh();
        }
        public void Refresh()
        {
            if (this.Selected)
            {
                if (this.DisplayImage != null) DisplayImage.Dispose();
                if (this.EditImage != null) EditImage.Dispose();
                Bitmap rchBox = RtbCopy();
                this.PSize = new Size(TextBox.Width + 2 * Margin + 2 * Padding, TextBox.Height + 2 * Margin + 2 * Padding);
                this.DisplayImage = new Bitmap(TextBox.Width + 2 * Padding + 2 * Margin, TextBox.Height + 2 * Padding + 2 * Margin);
                using (Graphics g = Graphics.FromImage(DisplayImage))
                {
                    g.DrawImage(rchBox, Margin + Padding, Margin + Padding);
                }
                rchBox.Dispose();
                this.EditImage = new Bitmap(DisplayImage.Width, DisplayImage.Height);
                using (Graphics g = Graphics.FromImage(EditImage))
                {
                    Pen p = new Pen(Color.Gray, 1);
                    SolidBrush sb = new SolidBrush(Color.Gray);
                    p.DashPattern = new float[] { 4, 2 };
                    g.DrawRectangle(p, new Rectangle(new Point(Margin, Margin), new Size(EditImage.Width - 2 * Margin, EditImage.Height - 2 * Margin)));
                    Rectangle square = new Rectangle(new Point(0, 0), new Size(SquareApothem * 2 + 1, SquareApothem * 2 + 1));
                    g.FillRectangle(sb, square);
                    square.Location = new Point(Image.Width - 2 * SquareApothem - 1, 0);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(Image.Width / 2 - SquareApothem, 0);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(0, Image.Height - 2 * SquareApothem - 1);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(Image.Width - 2 * SquareApothem - 1, Image.Height - 2 * SquareApothem - 1);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(Image.Width / 2 - SquareApothem, Image.Height - 2 * SquareApothem - 1);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(0, Image.Height / 2 - SquareApothem);
                    g.FillRectangle(sb, square);
                    square.Location = new Point(Image.Width - 2 * SquareApothem - 1, Image.Height / 2 - SquareApothem);
                    g.FillRectangle(sb, square);
                }
            }
        }
        // За да може текст от RTF формат да се прехвърли в Bitmap, се копират пикселите от полето, след което се
        // премахва белият фон
        public Bitmap RtbCopy()
        {
            TextBox.Update();
            Bitmap bmp = new Bitmap(TextBox.Width, TextBox.Height, PixelFormat.Format32bppArgb);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(TextBox.PointToScreen(Point.Empty), Point.Empty, TextBox.Size);
            }
            Bitmap output = ImageTools.MakeTransparent(bmp);
            bmp.Dispose();
            return output;
        }
    }
    public class TextButtons
    {
        public static Button Bold, Italic, UnderLine, Strike, Subscript, Superscript;
        public static ComboBox SizeBox, ColorBox;
        private static Color[] colors = new Color[] { Color.Black, Color.Red, Color.Green, Color.Blue };
        public static Color CurrentColor
        {
            get
            {
                return colors[ColorBox.SelectedIndex];
            }
        }

        public static void Load()
        {
            if (Bold != null) Bold.Click += BoldMethod;
            if (Italic != null) Italic.Click += ItalicMethod;
            if (UnderLine != null) UnderLine.Click += UnderLineMethod;
            if (Strike != null) Strike.Click += StrikeMethod;
            if (Subscript != null) Subscript.Click += SubscriptMethod;
            if (Superscript != null) Superscript.Click += SuperscriptMethod;
            if (ColorBox != null)
            {
                DisplayColorBox();
                ColorBox.SelectedIndex = 0;
            }
        }

        public static void DisplayColorBox()
        {
            ColorBox.DrawMode = DrawMode.OwnerDrawFixed;
            ColorBox.Items.Clear();
            foreach (Color color in colors) ColorBox.Items.Add(color);
            ColorBox.DrawItem += ColorDrawItem;
            ColorBox.SelectedIndexChanged += OnColorChange;
        }

        private const int MarginWidth = 0;
        private const int MarginHeight = 0;

        public static void ColorDrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            int hgt = e.Bounds.Height - 2 * MarginHeight;
            Rectangle rect = new Rectangle(
                e.Bounds.X + MarginWidth,
                e.Bounds.Y + MarginHeight,
                e.Bounds.Width, hgt);
            ComboBox cbo = sender as ComboBox;
            Color color = (Color)cbo.Items[e.Index];
            using (SolidBrush brush = new SolidBrush(color))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
            e.DrawFocusRectangle();
        }

        private static void OnColorChange(object sender, EventArgs e)
        {
            ComboBox cbo = sender as ComboBox;
            cbo.BackColor = colors[cbo.SelectedIndex];
        }

        private static void BoldMethod(Object sender, EventArgs e)
        {
            TextSelect.BoldSelected();
        }

        private static void ItalicMethod(Object sender, EventArgs e)
        {
            TextSelect.ItalicSelected();
        }

        private static void UnderLineMethod(Object sender, EventArgs e)
        {
            TextSelect.UnderlineSelected();
        }

        private static void StrikeMethod(Object sender, EventArgs e)
        {
            TextSelect.StrikeSelected();
        }

        private static void SubscriptMethod(Object sender, EventArgs e)
        {
            TextSelect.SubscriptSelected();
        }

        private static void SuperscriptMethod(Object sender, EventArgs e)
        {
            TextSelect.SuperscriptSelected();
        }

        public static void EnableAll()
        {
            if (Bold != null) Bold.Enabled = true;
            if (Italic != null) Italic.Enabled = true;
            if (UnderLine != null) UnderLine.Enabled = true;
            if (Strike != null) Strike.Enabled = true;
            if (Subscript != null) Subscript.Enabled = true;
            if (Superscript != null) Superscript.Enabled = true;
        }

        public static void DisableAll()
        {
            if (Bold != null) Bold.Enabled = false;
            if (Italic != null) Italic.Enabled = false;
            if (UnderLine != null) UnderLine.Enabled = false;
            if (Strike != null) Strike.Enabled = false;
            if (Subscript != null) Subscript.Enabled = false;
            if (Superscript != null) Superscript.Enabled = false;
        }

        public static void SelectedDisable()
        {
            if (SizeBox != null) SizeBox.Enabled = false;
            if (ColorBox != null) ColorBox.Enabled = false;
        }

        public static void UnselectedEnable()
        {
            if (SizeBox != null) SizeBox.Enabled = true;
            if (ColorBox != null) ColorBox.Enabled = true;
        }
    }
    public class TextLayers
    {
        public static int MinSize = 48;
        public static Size DefaultSize = new Size(300, 35);
        public static double OffsetCoef = 0.5;
        public static Size FullSize;

        public static TextLayer[] Layers;
        public static int CurrentNumberOfLayers = 0;
        public static PictureBox Container;
        public static PictureBox TopContainer;

        private static int LayerNumber = 0;

        public static void Initialize(int MaxLayers, PictureBox Container, PictureBox TopContainer)
        {
            Layers = new TextLayer[MaxLayers];
            TextLayers.Container = Container;
            TextLayers.TopContainer = TopContainer;
        }

        public static void Refresh()
        {
            ImageLayers.Refresh();
            Container.Refresh();
        }

        public static bool AddLayer(Point Location, Size Size, int FontSize)
        {
            if (CurrentNumberOfLayers < Layers.Length)
            {
                RichTextBox rtb = new RichTextBox();
                rtb.Location = Location;
                rtb.Name = "InnerRTB" + LayerNumber.ToString();
                LayerNumber++;
                rtb.Size = Size;
                rtb.Font = new Font(rtb.Font.FontFamily, FontSize);
                rtb.ForeColor = TextButtons.CurrentColor;
                rtb.Text = "";
                rtb.Show();
                rtb.Parent = TopContainer;
                rtb.WordWrap = true;
                rtb.BorderStyle = BorderStyle.None;
                rtb.ScrollBars = RichTextBoxScrollBars.None;
                rtb.ContentsResized += RichTextBoxResizeHandle;
                rtb.SelectionChanged += RichTextBoxSelectionHandle;
                TopContainer.Controls.Add(rtb);
                rtb.Focus();
                Layers[CurrentNumberOfLayers] = new TextLayer(Location, rtb);
                TextSelect.Activate(CurrentNumberOfLayers);
                CurrentNumberOfLayers++;
                Reload();
                return true;
            }
            else return false;
        }

        public static void RichTextBoxSelectionHandle(object sender, EventArgs e)
        {
            RichTextBox rch = sender as RichTextBox;
            if (rch.SelectionLength == 0) TextButtons.DisableAll();
            else TextButtons.EnableAll();
        }
        // Този метод подсигурява, че винаги полето ще побира текста
        public static void RichTextBoxResizeHandle(object sender, ContentsResizedEventArgs e)
        {
            RichTextBox rch = sender as RichTextBox;
            int margin = rch.Margin.Top;
            rch.ClientSize = new Size(rch.ClientSize.Width, e.NewRectangle.Height + margin);
            TextLayers.Reload();
            TextSelect.SetCursorSquares();
        }

        public static void DeleteLayer(int Layer)
        {

            if (Layer < CurrentNumberOfLayers)
            {
                TextSelect.FinalDeactivate();
                Layers[Layer].Clear();
                CurrentNumberOfLayers--;
                for (int i = Layer; i < CurrentNumberOfLayers; i++) Layers[i] = Layers[i + 1];
                Reload();
            }

        }

        public static int LayerOnCoordinates(Point Loc)
        {
            int output = -1;
            for (int i = CurrentNumberOfLayers - 1; i >= 0; i--)
            {
                TextLayer tb = Layers[i];
                Point Location = tb.Location;
                if (Loc.X >= Location.X && Loc.X < Location.X + tb.Width && Loc.Y >= Location.Y && Loc.Y < Location.Y + tb.Height)
                {
                    output = i;
                    break;
                }
            }
            return output;
        }

        public static void Reload()
        {
            for (int i = 0; i < CurrentNumberOfLayers; i++) Layers[i].Refresh();
            Container.Image.Dispose();
            Container.Image = DrawLayers();
        }

        public static Bitmap DrawLayers()
        {
            Bitmap output = new Bitmap(FullSize.Width, FullSize.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.Clear(Color.Transparent);
                for (int i = 0; i < CurrentNumberOfLayers; i++) g.DrawImage(Layers[i].Image, new Rectangle(Layers[i].Location, Layers[i].Size));

            }
            return output;
        }
    }
    public class TextSelect
    {
        public static bool Active = false;
        private volatile static bool Wait = false;
        public static bool ActiveResize = false;
        public static int Layer = -1;
        public static CursorSquare[] CursorSquares = new CursorSquare[8];
        public static int CurrentSquareIndex = -1;

        public static void Activate(int Layer)
        {
            TextSelect.Active = true;
            TextButtons.SelectedDisable();
            TextSelect.Layer = Layer;
            TextLayers.Layers[Layer].Selected = true;
            TextLayers.Layers[Layer].TextBox.Visible = true;
            TextLayers.Layers[Layer].TextBox.Focus();
            SetCursorSquares();
            TextLayers.Reload();
        }

        public static void SetCursorSquares()
        {
            int SquareApothem = TextLayer.SquareApothem;
            int Margin = TextLayer.Margin;
            TextLayer Text = TextLayers.Layers[Layer];
            CursorSquares[0] = new CursorSquare(new Point(SquareApothem, SquareApothem), SquareApothem, Cursors.SizeNWSE);
            CursorSquares[1] = new CursorSquare(new Point(Text.Width / 2, SquareApothem), SquareApothem, Cursors.SizeNS);
            CursorSquares[2] = new CursorSquare(new Point(Text.Width - SquareApothem - Margin, SquareApothem), SquareApothem, Cursors.SizeNESW);
            CursorSquares[3] = new CursorSquare(new Point(Text.Width - SquareApothem - Margin, Text.Height / 2), SquareApothem, Cursors.SizeWE);
            CursorSquares[4] = new CursorSquare(new Point(Text.Width - SquareApothem - Margin, Text.Height - SquareApothem - Margin), SquareApothem, Cursors.SizeNWSE);
            CursorSquares[5] = new CursorSquare(new Point(Text.Width / 2, Text.Height - SquareApothem - Margin), SquareApothem, Cursors.SizeNS);
            CursorSquares[6] = new CursorSquare(new Point(SquareApothem, Text.Height - SquareApothem - Margin), SquareApothem, Cursors.SizeNESW);
            CursorSquares[7] = new CursorSquare(new Point(SquareApothem, Text.Height / 2), SquareApothem, Cursors.SizeWE);
        }

        public static Rectangle GenerateClip(LocationAndSize Container)
        {
            int SquareApothem = TextLayer.SquareApothem;
            int Margin = TextLayer.Padding;
            int MinSize = TextLayers.MinSize;
            TextLayer Text = TextLayers.Layers[Layer];
            LocationAndSize RectData = new LocationAndSize();
            Point Loc = Container.Location;
            Size ContSize = Container.Size;
            switch (CurrentSquareIndex)
            {
                case 0:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y - Margin - SquareApothem), new Point(Loc.X + Text.Location.X + Text.Width - MinSize, Loc.Y + Text.Location.Y + Text.Height - MinSize));
                    break;
                case 1:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + Text.Location.Y + Text.Height - MinSize), new Point(Loc.X + ContSize.Width + 2 * Margin + 2 * SquareApothem, Loc.Y - Margin - SquareApothem));
                    break;
                case 2:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Text.Location.X + MinSize, Loc.Y + Text.Location.Y + Text.Height - MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y - Margin - SquareApothem));
                    break;
                case 3:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Text.Location.X + MinSize, Loc.Y - Margin - SquareApothem), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 4:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X + Text.Location.X + MinSize, Loc.Y + Text.Location.Y + MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 5:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + Text.Location.Y + MinSize), new Point(Loc.X + ContSize.Width + Margin + SquareApothem, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
                case 6:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y + ContSize.Width + Margin + SquareApothem), new Point(Loc.X + Text.Location.X + Text.Width - MinSize, Loc.Y + Text.Location.Y + MinSize));
                    break;
                case 7:
                    RectData = LocationAndSize.FromPoints(new Point(Loc.X - Margin - SquareApothem, Loc.Y - Margin - SquareApothem), new Point(Loc.X + Text.Location.X + Text.Width - MinSize, Loc.Y + ContSize.Height + Margin + SquareApothem));
                    break;
            }
            return RectData.ToRectangle();
        }

        public static void Resize(Point MousePoint)
        {
            Cursor.Current = CursorSquares[CurrentSquareIndex].Cursor;
            TextLayer Text = TextLayers.Layers[Layer];
            LocationAndSize TextPosition = new LocationAndSize();
            Point CurrentAPoint = Text.Location;
            Point CurrentBPoint = new Point(CurrentAPoint.X + Text.Width - 1, CurrentAPoint.Y + Text.Height - 1);
            int OWidth = Text.Width;
            Text.TextBox.Visible = false;

            switch (CurrentSquareIndex)
            {
                case 0:
                    TextPosition = LocationAndSize.FromPoints(MousePoint, CurrentBPoint);
                    break;
                case 1:
                    TextPosition = LocationAndSize.FromPoints(new Point(CurrentAPoint.X, MousePoint.Y), CurrentBPoint);
                    break;
                case 2:
                    TextPosition = LocationAndSize.FromPoints(new Point(CurrentAPoint.X, CurrentBPoint.Y), MousePoint);
                    break;
                case 3:
                    TextPosition = LocationAndSize.FromPoints(new Point(MousePoint.X, CurrentBPoint.Y), CurrentAPoint);
                    break;
                case 4:
                    TextPosition = LocationAndSize.FromPoints(CurrentAPoint, MousePoint);
                    break;
                case 5:
                    TextPosition = LocationAndSize.FromPoints(new Point(CurrentBPoint.X, MousePoint.Y), CurrentAPoint);
                    break;
                case 6:
                    TextPosition = LocationAndSize.FromPoints(new Point(CurrentBPoint.X, CurrentAPoint.Y), MousePoint);
                    break;
                case 7:
                    TextPosition = LocationAndSize.FromPoints(new Point(MousePoint.X, CurrentAPoint.Y), CurrentBPoint);
                    break;
                default:
                    TextPosition = LocationAndSize.FromPoints(CurrentAPoint, CurrentBPoint);
                    break;
            }

            Text.Location = TextPosition.Location;
            Text.Size = TextPosition.Size;
            Text.Refresh();
            TextLayers.Reload();
            Text.TextBox.Visible = true;
        }

        public static void BoldSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            if (tl.TextBox.SelectionFont != null)
            {
                if (!tl.TextBox.SelectionFont.Bold) tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, FontStyle.Bold | tl.TextBox.SelectionFont.Style);
                else tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, ~FontStyle.Bold & tl.TextBox.SelectionFont.Style);
                int selstart = tl.TextBox.SelectionStart;
                int sellength = tl.TextBox.SelectionLength;
                tl.TextBox.SelectionStart = tl.TextBox.SelectionStart + tl.TextBox.SelectionLength;
                tl.TextBox.SelectionLength = 0;
                tl.TextBox.SelectionFont = tl.TextBox.Font;
                tl.TextBox.Select(selstart, sellength);
                tl.TextBox.Focus();
            }
        }

        public static void ItalicSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            if (tl.TextBox.SelectionFont != null)
            {
                if (!tl.TextBox.SelectionFont.Italic) tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, FontStyle.Italic | tl.TextBox.SelectionFont.Style);
                else tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, ~FontStyle.Italic & tl.TextBox.SelectionFont.Style);
                int selstart = tl.TextBox.SelectionStart;
                int sellength = tl.TextBox.SelectionLength;
                tl.TextBox.SelectionStart = tl.TextBox.SelectionStart + tl.TextBox.SelectionLength;
                tl.TextBox.SelectionLength = 0;
                tl.TextBox.SelectionFont = tl.TextBox.Font;
                tl.TextBox.Select(selstart, sellength);
                tl.TextBox.Focus();
            }
        }

        public static void UnderlineSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            if (tl.TextBox.SelectionFont != null)
            {
                if (!tl.TextBox.SelectionFont.Underline) tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, FontStyle.Underline | tl.TextBox.SelectionFont.Style);
                else tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, ~FontStyle.Underline & tl.TextBox.SelectionFont.Style);
                int selstart = tl.TextBox.SelectionStart;
                int sellength = tl.TextBox.SelectionLength;
                tl.TextBox.SelectionStart = tl.TextBox.SelectionStart + tl.TextBox.SelectionLength;
                tl.TextBox.SelectionLength = 0;
                tl.TextBox.SelectionFont = tl.TextBox.Font;
                tl.TextBox.Select(selstart, sellength);
                tl.TextBox.Focus();
            }
        }

        public static void StrikeSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            if (tl.TextBox.SelectionFont != null)
            {
                if (!tl.TextBox.SelectionFont.Strikeout) tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, FontStyle.Strikeout | tl.TextBox.SelectionFont.Style);
                else tl.TextBox.SelectionFont = new Font(tl.TextBox.Font, ~FontStyle.Strikeout & tl.TextBox.SelectionFont.Style);
                int selstart = tl.TextBox.SelectionStart;
                int sellength = tl.TextBox.SelectionLength;
                tl.TextBox.SelectionStart = tl.TextBox.SelectionStart + tl.TextBox.SelectionLength;
                tl.TextBox.SelectionLength = 0;
                tl.TextBox.SelectionFont = tl.TextBox.Font;
                tl.TextBox.Select(selstart, sellength);
                tl.TextBox.Focus();
            }
        }

        public static bool IsSubscript()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            int selstart = tl.TextBox.SelectionStart;
            int sellength = tl.TextBox.SelectionLength;
            bool output = true;
            for (int i = selstart; output && i < selstart + sellength; i++)
            {
                tl.TextBox.Select(i, 1);
                output = tl.TextBox.SelectionCharOffset == -(int)(TextLayers.OffsetCoef * tl.OriginalFontSize);
            }
            tl.TextBox.Select(selstart, sellength);
            return output;
        }

        public static bool IsSuperscript()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            int selstart = tl.TextBox.SelectionStart;
            int sellength = tl.TextBox.SelectionLength;
            bool output = true;
            for (int i = selstart; output && i < selstart + sellength; i++)
            {
                tl.TextBox.Select(i, 1);
                output = tl.TextBox.SelectionCharOffset == (int)(TextLayers.OffsetCoef * tl.OriginalFontSize);
            }
            tl.TextBox.Select(selstart, sellength);
            return output;
        }

        public static void SubscriptSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            Font NewFont = new Font(tl.TextBox.Font.FontFamily, tl.OriginalFontSize);
            int selstart = tl.TextBox.SelectionStart;
            int sellength = tl.TextBox.SelectionLength;
            int offset = 0;
            bool iss = IsSubscript();

            if (!iss)
            {
                NewFont = new Font(NewFont.FontFamily, NewFont.Size * 0.75f);
                offset = -(int)(tl.OriginalFontSize * TextLayers.OffsetCoef);
            }

            tl.TextBox.SelectionCharOffset = offset;
            tl.TextBox.SelectionFont = NewFont;

            if (selstart + sellength == tl.TextBox.TextLength && !iss)
            {
                tl.TextBox.Select(selstart + sellength, 1);
                tl.TextBox.SelectedText = " ";
                tl.TextBox.Select(selstart + sellength, 2);
                tl.TextBox.SelectionCharOffset = 0;
                tl.TextBox.SelectionFont = new Font(tl.TextBox.Font.FontFamily, tl.OriginalFontSize);
            }

            tl.TextBox.Focus();
            tl.TextBox.Select(selstart, sellength);
        }

        public static void SuperscriptSelected()
        {
            TextLayer tl = TextLayers.Layers[Layer];
            Font NewFont = new Font(tl.TextBox.Font.FontFamily, tl.OriginalFontSize);
            int selstart = tl.TextBox.SelectionStart;
            int sellength = tl.TextBox.SelectionLength;
            int offset = 0;
            bool iss = IsSuperscript();

            if (!iss)
            {
                NewFont = new Font(NewFont.FontFamily, NewFont.Size * 0.75f);
                offset = (int)(tl.OriginalFontSize * TextLayers.OffsetCoef);
            }

            tl.TextBox.SelectionCharOffset = offset;
            tl.TextBox.SelectionFont = NewFont;

            if (selstart + sellength == tl.TextBox.TextLength && !iss)
            {
                tl.TextBox.Select(selstart + sellength, 1);
                tl.TextBox.SelectedText = " ";
                tl.TextBox.Select(selstart + sellength, 2);
                tl.TextBox.SelectionCharOffset = 0;
                tl.TextBox.SelectionFont = new Font(tl.TextBox.Font.FontFamily, tl.OriginalFontSize);
            }

            tl.TextBox.Focus();
            tl.TextBox.Select(selstart, sellength);
        }

        public static void Deactivate()
        {
            if (TextLayers.Layers[Layer].TextBox.TextLength == 0) TextLayers.DeleteLayer(Layer);
            else FinalDeactivate();
        }

        public static void FinalDeactivate()
        {
            TextSelect.Active = false;
            TextLayers.TopContainer.Focus();
            Thread th = new Thread(TextSelect.WaitScreen);
            th.Start();

            while (Wait)
            {
                // WAIT; изчакването е нужно, за да се актуализират пикселите по текстовото поле
            }

            TextLayers.Layers[Layer].Refresh();
            TextLayers.Layers[Layer].TextBox.Visible = false;
            TextLayers.Layers[Layer].Selected = false;
            TextLayers.Layers[Layer].TextBox.Select(TextLayers.Layers[Layer].TextBox.TextLength, 0);
            TextButtons.DisableAll();
            TextButtons.UnselectedEnable();
            TextLayers.Reload();
        }

        public static int SquareIndex(Point Location)
        {
            Point InImageLocation = new Point(Location.X - TextLayers.Layers[Layer].Location.X, Location.Y - TextLayers.Layers[Layer].Location.Y);
            int output = 0;
            while (output != -1)
            {
                if (output == CursorSquares.Length) output = -1;
                else if (CursorSquares[output].IsPointInside(InImageLocation)) break;
                else output++;
            }
            return output;
        }

        public static bool IsInDragZone(Point Location)
        {
            bool output = false;
            TextLayer tl = TextLayers.Layers[Layer];
            int margin = TextLayer.Margin;
            if (SquareIndex(Location) == -1 && TextLayers.LayerOnCoordinates(Location) == Layer)
            {
                output = (Location.X < tl.Location.X + 2 * margin ||
                          Location.Y < tl.Location.Y + 2 * margin ||
                          Location.X > tl.Location.X + tl.Width - 2 * margin ||
                          Location.Y > tl.Location.Y + tl.Height - 2 * margin);
            }
            return output;
        }

        private static void WaitScreen()
        {
            TextSelect.Wait = true;
            Thread.Sleep(50);
            TextSelect.Wait = false;
        }
    }
    public class TextDrag
    {
        public static bool Active = false;
        public static int initX, initY, initLocX, initLocY, Layer;

        public static void Activate(int Layer, Point Location)
        {
            TextDrag.Active = true;
            TextDrag.Layer = Layer;
            TextDrag.initX = Location.X;
            TextDrag.initY = Location.Y;
            TextDrag.initLocX = TextLayers.Layers[Layer].Location.X;
            TextDrag.initLocY = TextLayers.Layers[Layer].Location.Y;
            TextLayers.Layers[Layer].TextBox.Hide();
        }

        public static void Update(Point Location)
        {
            TextLayers.Layers[Layer].Location = new Point(initLocX + Location.X - initX, initLocY + Location.Y - initY);
            TextLayers.Reload();
        }

        public static void Deactivate()
        {
            Active = false;
            TextLayers.Layers[Layer].TextBox.Show();
        }
    }
    public class PenTool
    {
        public static bool Active = false;
        public static bool Sponge = false;
        public static int initX = 0, initY = 0;
        public static float PenSize;
        public static Color PenColor
        {
            get
            {
                return PenC;
            }

            set
            {
                if (value == EraserColor) PenC = ReplacerColor;
                else PenC = value;
            }
        }
        private static Color PenC;
        public static Bitmap PenContent;
        public static PictureBox PenContainer;
        public static Color EraserColor = Color.DarkMagenta, ReplacerColor = Color.Magenta;

        public static void Initialize(PictureBox PenContainer, float PenSize, Color PenColor, Size FullSize)
        {
            PenTool.PenSize = PenSize;
            PenTool.PenContainer = PenContainer;
            PenTool.PenColor = PenColor;
            Bitmap Background = new Bitmap(FullSize.Width, FullSize.Height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(Background)) g.Clear(Color.Transparent);
            PenContainer.Image = Background;
        }

        public static void StartDrawing(Point Location)
        {
            Active = true;
            Color c = Sponge ? EraserColor : PenColor;
            SolidBrush sb = new SolidBrush(c);
            using (Graphics g = Graphics.FromImage(PenContainer.Image)) g.FillEllipse(sb, Location.X - (int)PenSize / 2, Location.Y - (int)PenSize / 2, PenSize, PenSize);
            initX = Location.X;
            initY = Location.Y;
            if (Sponge) (PenContainer.Image as Bitmap).MakeTransparent(EraserColor);
            TextLayers.Refresh();
        }

        public static void DrawMove(Point Location)
        {
            Color c = Sponge ? EraserColor : PenColor;
            Pen p = new Pen(c, PenSize);
            SolidBrush sb = new SolidBrush(c);
            using (Graphics g = Graphics.FromImage(PenContainer.Image))
            {
                g.DrawLine(p, initX, initY, Location.X, Location.Y);
                g.FillEllipse(sb, initX - (int)PenSize / 2, initY - (int)PenSize / 2, PenSize, PenSize);
                g.FillEllipse(sb, Location.X - (int)PenSize / 2, Location.Y - (int)PenSize / 2, PenSize, PenSize);
            }
            // Гъбата за маркер работи по следния начин: чертае се с определен цвят, който иначе е невъзможен за
            // цвят на маркера, след което с метода MakeTransparent() на класа Bitmap този цвят се премахва от 
            // изображението.
            if (Sponge) (PenContainer.Image as Bitmap).MakeTransparent(EraserColor);
            initX = Location.X;
            initY = Location.Y;
            Refresh();
        }
        public static void StopDrawing()
        {
            Active = false;
        }
        public static void Clear()
        {
            using (Graphics g = Graphics.FromImage(PenContainer.Image)) g.Clear(Color.Transparent);
            TextLayers.Refresh();
        }

        public static void Refresh()
        {
            PenContainer.Refresh();
            TextLayers.Refresh();
        }
    }



    // Геометрични класове
    // Основата на тези класове е идеята, че правите в равнината може да се представят като графики на
    // линейни функции; пресичането на графиките на функциите f(x) и g(x) е при решението на f(x)=g(x);
    // Графиките на две линейни функции са успоредни тогава и само тогава, когато имат равни ъглови 
    // коефициенти и са перпендикулярни тогава и само тогава, когато произведението от ъгловите им 
    // коефициенти е -1.

    public class GeometryTools
    {
        public static bool Loaded { get; private set; }
        public static Panel MenuContainer;
        public static PictureBox Container, PresentationContainer;
        public static RadioButton LineButton, AltitudeButton, CircleButton, MiddleButton, CCircleButton, RectangleButton;
        private static GeoTool GTool = GeoTool.None;
        public static PointSelection ps;
        public static void Load()
        {
            LineButton.CheckedChanged += LineMethod;
            AltitudeButton.CheckedChanged += AltitudeMethod;
            CircleButton.CheckedChanged += CircleMethod;
            MiddleButton.CheckedChanged += MiddleMethod;
            CCircleButton.CheckedChanged += CCircleMethod;
            RectangleButton.CheckedChanged += RectangleMethod;
            ps = PointSelection.Empty;
            Loaded = true;
        }

        private static void LineMethod(object sender, EventArgs e)
        {
            if (LineButton.Checked)
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.Line;
                ActionCallback();
            }
        }
        private static void AltitudeMethod(object sender, EventArgs e)
        {
            if (!AltitudeButton.Checked) GTool = GeoTool.None;
            else
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.Altitude;
                ActionCallback();
            }
        }
        private static void CircleMethod(object sender, EventArgs e)
        {
            if (!CircleButton.Checked) GTool = GeoTool.None;
            else
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.Circle;
                ActionCallback();
            }
        }
        private static void MiddleMethod(object sender, EventArgs e)
        {
            if (!MiddleButton.Checked) GTool = GeoTool.None;
            else
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.Middle;
                ActionCallback();
            }
        }
        private static void CCircleMethod(object sender, EventArgs e)
        {
            if (!CCircleButton.Checked) GTool = GeoTool.None;
            else
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.CircumscribedCircle;
                ActionCallback();
            }
        }
        private static void RectangleMethod(object sender, EventArgs e)
        {
            if (!RectangleButton.Checked) GTool = GeoTool.None;
            else
            {
                Tool.Current = Tools.Geometry;
                GTool = GeoTool.Rectangle;
                ActionCallback();
            }
        }
        public static void DisableButtons()
        {
            if (Loaded)
            {
                GTool = GeoTool.None;
                ps.End();
                foreach (Control control in MenuContainer.Controls)
                {
                    if (control is RadioButton chk) chk.Checked = false;
                }
            }
        }
        private static int NumOfPoints
        {
            get
            {
                if (GTool == GeoTool.Line) return 2;
                if (GTool == GeoTool.Altitude) return 3;
                if (GTool == GeoTool.Circle) return 2;
                if (GTool == GeoTool.Middle) return 2;
                if (GTool == GeoTool.CircumscribedCircle) return 3;
                if (GTool == GeoTool.Rectangle) return 2;
                return 0;
            }
        }
        private static void ActionCallback()
        {
            ps.End();
            ps = new PointSelection(NumOfPoints);
            using (Graphics g = Graphics.FromImage(PresentationContainer.Image)) g.Clear(Color.Transparent);
            if (GTool == GeoTool.Line)
            {
                ps.PointsSelected += DrawLine;
                ps.LastPointEvent += PresentationDrawLine;
            }
            if (GTool == GeoTool.Altitude)
            {
                ps.PointsSelected += DrawAltitude;
                ps.LastPointEvent += PresentationDrawAltitude;
            }
            if (GTool == GeoTool.Circle)
            {
                ps.PointsSelected += DrawCircle;
                ps.LastPointEvent += PresentationDrawCircle;
            }
            if (GTool == GeoTool.Middle)
            {
                ps.PointsSelected += DrawMiddle;
                ps.LastPointEvent += PresentationDrawMiddle;
            }
            if (GTool == GeoTool.CircumscribedCircle)
            {
                ps.PointsSelected += DrawCircumscribedCircle;
                ps.LastPointEvent += PresentationDrawCircumscribedCircle;
            }
            if (GTool == GeoTool.Rectangle)
            {
                ps.PointsSelected += DrawRectangle;
                ps.LastPointEvent += PresentationDrawRectangle;
            }
            ps.Begin();
        }
        private static void DrawLine(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                using (Graphics g = Graphics.FromImage(Container.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    g.DrawLine(p, Points[0], Points[1]);
                }
                Container.Refresh();
            }
            ActionCallback();
        }
        private static void PresentationDrawLine(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    g.Clear(Color.Transparent);
                    g.DrawLine(p, Points[0], Points[1]);
                }
                PresentationContainer.Refresh();
            }
        }
        private static void DrawAltitude(Point[] Points)
        {
            if (Points.Length >= 3)
            {
                Line l = Line.FromPoints(Points[0], Points[1]);
                Line al = l.Altitude(Points[2]);
                if (al.Type != LineType.NonExistent)
                {
                    using (Graphics g = Graphics.FromImage(Container.Image))
                    {
                        Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                        g.DrawLine(p, al.Points[0], al.Points[1]);
                    }
                    Container.Refresh();
                }
            }
            ActionCallback();
        }
        private static void PresentationDrawAltitude(Point[] Points)
        {
            if (Points.Length >= 3)
            {
                Line l = Line.FromPoints(Points[0], Points[1]);
                Line al = l.Altitude(Points[2]);
                if (al.Type != LineType.NonExistent)
                {
                    using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                    {
                        Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                        g.Clear(Color.Transparent);
                        g.DrawLine(p, al.Points[0], al.Points[1]);
                    }
                    PresentationContainer.Refresh();
                }
            }
        }
        private static void DrawCircle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                int r = Distance(Points[0], Points[1]);
                Circle c = new Circle(Points[0], r);
                using (Graphics g = Graphics.FromImage(Container.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    g.DrawEllipse(p, c.ToRectangle());
                }
                Container.Refresh();
            }
            ActionCallback();
        }
        private static void PresentationDrawCircle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                int r = Distance(Points[0], Points[1]);
                Circle c = new Circle(Points[0], r);
                using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    g.Clear(Color.Transparent);
                    g.DrawEllipse(p, c.ToRectangle());
                }
                PresentationContainer.Refresh();
            }
        }
        private static void DrawMiddle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                Line l = Line.FromPoints(Points[0], Points[1]);
                Point M = l.Midpoint;
                using (Graphics g = Graphics.FromImage(Container.Image))
                {
                    SolidBrush sb = new SolidBrush(PenTool.PenColor);
                    Circle c = new Circle(M, (int)(PenTool.PenSize * (0.6)) + 2);
                    g.FillEllipse(sb, c.ToRectangle());
                }
                Container.Refresh();
            }
            ActionCallback();
        }
        private static void PresentationDrawMiddle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                Line l = Line.FromPoints(Points[0], Points[1]);
                Point M = l.Midpoint;
                using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                {
                    SolidBrush sb = new SolidBrush(PenTool.PenColor);
                    Circle c = new Circle(M, (int)(PenTool.PenSize * (0.6)) + 2);
                    g.Clear(Color.Transparent);
                    g.FillEllipse(sb, c.ToRectangle());
                }
                PresentationContainer.Refresh();
            }
        }
        private static void DrawCircumscribedCircle(Point[] Points)
        {
            if (Points.Length >= 3)
            {
                Line a = Line.FromPoints(Points[0], Points[1]);
                Line b = Line.FromPoints(Points[1], Points[2]);
                Intersection isec = Line.IntersectLines(a.Bisector(), b.Bisector());
                if (isec.PointExists) using (Graphics g = Graphics.FromImage(Container.Image))
                    {
                        Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                        Circle c = new Circle(isec.Location, Distance(Points[2], isec.Location));
                        g.DrawEllipse(p, c.ToRectangle());
                    }
                PresentationContainer.Refresh();
            }
            ActionCallback();
        }
        private static void PresentationDrawCircumscribedCircle(Point[] Points)
        {
            if (Points.Length >= 3)
            {
                Line a = Line.FromPoints(Points[0], Points[1]);
                Line b = Line.FromPoints(Points[1], Points[2]);
                Intersection isec = Line.IntersectLines(b.Bisector(), a.Bisector());
                if (isec.PointExists) using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                    {
                        Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                        SolidBrush sb = new SolidBrush(PenTool.PenColor);
                        Circle c = new Circle(isec.Location, Distance(Points[2], isec.Location));
                        g.Clear(Color.Transparent);
                        g.DrawEllipse(p, c.ToRectangle());
                    }
                PresentationContainer.Refresh();
            }
        }
        private static void DrawRectangle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                using (Graphics g = Graphics.FromImage(Container.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    LocationAndSize las = LocationAndSize.FromPoints(Points[0], Points[1]);
                    Rectangle r = las.ToRectangle();
                    g.DrawRectangle(p, r);
                }
                Container.Refresh();
            }
            ActionCallback();
        }
        private static void PresentationDrawRectangle(Point[] Points)
        {
            if (Points.Length >= 2)
            {
                using (Graphics g = Graphics.FromImage(PresentationContainer.Image))
                {
                    Pen p = new Pen(PenTool.PenColor, PenTool.PenSize);
                    LocationAndSize las = LocationAndSize.FromPoints(Points[0], Points[1]);
                    Rectangle r = las.ToRectangle();
                    g.Clear(Color.Transparent);
                    g.DrawRectangle(p, r);
                }
                PresentationContainer.Refresh();
            }
        }
        private static int Distance(Point A, Point B)
        {
            return (int)Math.Round(Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2)));
        }

    }
    public class GeoTool
    {
        public static GeoTool None = new GeoTool();
        public static GeoTool Line = new GeoTool();
        public static GeoTool Altitude = new GeoTool();
        public static GeoTool Circle = new GeoTool();
        public static GeoTool Middle = new GeoTool();
        public static GeoTool CircumscribedCircle = new GeoTool();
        public static GeoTool Rectangle = new GeoTool();
    }
    public class Interval
    {
        public double Start, End;

        public Interval(double Start, double End)
        {
            this.Start = Start;
            this.End = End;
        }
    }
    public class Circle
    {
        public Point Center;
        public int Radius;

        public Circle(Point Center, int Radius)
        {
            this.Center = Center;
            this.Radius = Radius;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(new Point(Center.X - Radius, Center.Y - Radius), new Size(2 * Radius, 2 * Radius));
        }
    }
    public class Line
    {
        public LineType Type = LineType.NonExistent;
        // Старши коеф. при декартови прави; Празно при вертикална
        public double A;
        // Свободен член при декартови прави; Абсциса на вертикална
        public double B;

        // Интервал, крайни точки и среда - само при отсечки
        // При декартови прави интервал от абсцисата; При вертикални - от ординатата
        public Interval Interval;
        public Point[] Points;
        public Point Midpoint
        {
            get
            {
                int X = (int)Math.Round((double)(((double)Points[0].X + (double)Points[1].X) / 2));
                int Y = (int)Math.Round((double)(((double)Points[0].Y + (double)Points[1].Y) / 2));
                return new Point(X, Y);
            }
        }

        public double ValueAt(double x)
        {
            return A * x + B;
        }

        public static Line FromPoints(Point A, Point B)
        {
            Line output = new Line();
            output.Points = new Point[] { A, B };

            if (A.X == B.X)
            {
                output.Type = LineType.Vertical;
                output.B = A.X;
                output.Interval = new Interval(Math.Min(A.Y, B.Y), Math.Max(A.Y, B.Y));
            }

            else
            {
                output.Type = LineType.Cartesian;
                output.A = (double)(A.Y - B.Y) / (double)(A.X - B.X);
                output.B = (A.Y - output.A * A.X);
                output.Interval = new Interval(Math.Min(A.X, B.X), Math.Max(A.X, B.X));
            }

            return output;
        }

        public bool IsPointOnLine(Point C)
        {
            if (this.Type == LineType.Vertical) return C.X == this.B;
            else return C.Y == this.ValueAt(C.X);
        }
        public Line Bisector()
        {
            Line output = new Line();
            Point C = this.Midpoint;

            if (this.Type == LineType.Vertical)
            {
                output.Type = LineType.Cartesian;
                output.A = 0;
                output.B = (double)C.Y;
            }

            else if (this.Type == LineType.Cartesian && this.A == 0)
            {
                output.Type = LineType.Vertical;
                output.B = (double)C.X;
            }

            else
            {
                output.Type = LineType.Cartesian;
                output.A = -(1 / this.A);
                output.B = (double)C.X / this.A + (double)C.Y;
            }

            return output;
        }
        public Line Altitude(Point C)
        {
            Line output = new Line();

            if (this.IsPointOnLine(C)) output.Type = LineType.NonExistent;

            else if (this.Type == LineType.Vertical)
            {
                output.Type = LineType.Cartesian;
                output.A = 0;
                output.B = C.Y;
                output.Interval = new Interval(Math.Min(C.X, this.B), Math.Max(C.X, this.B));
                output.Points = new Point[] { C, new Point((int)this.B, C.Y) };
            }

            else if (this.Type == LineType.Cartesian && this.A == 0)
            {
                output.Type = LineType.Vertical;
                output.B = C.X;
                output.Interval = new Interval(Math.Min(C.Y, this.B), Math.Max(C.Y, this.B));
                output.Points = new Point[] { C, new Point(C.X, (int)this.B) };
            }

            else
            {
                output.Type = LineType.Cartesian;
                output.A = -(1 / this.A);
                output.B = C.X / this.A + C.Y;
                Intersection isec = Line.IntersectLines(this, output);
                if (isec.PointExists) output.Interval = new Interval(Math.Min(C.X, isec.Location.X), Math.Max(C.X, isec.Location.X));
                output.Points = new Point[] { C, isec.Location };
            }

            return output;
        }

        public static Intersection IntersectLines(Line A, Line B)
        {
            Intersection output = new Intersection();
            if (A.Type == LineType.Vertical && B.Type == LineType.Vertical) output.PointExists = false;
            else if (A.Type == LineType.Cartesian && B.Type == LineType.Cartesian && A.A == B.A) output.PointExists = false;
            else output.PointExists = true;

            if (A.Type == LineType.Cartesian && B.Type == LineType.Vertical) output.Location = new Point((int)B.B, (int)A.ValueAt(B.B));
            if (A.Type == LineType.Vertical && B.Type == LineType.Cartesian) output.Location = new Point((int)A.B, (int)B.ValueAt(A.B));
            if (A.Type == LineType.Cartesian && B.Type == LineType.Cartesian)
            {

                int x = (int)Math.Round(((double)(B.B - A.B) / (double)(A.A - B.A)));
                if (Math.Abs(A.A) > Math.Abs(B.A)) output.Location = new Point(x, (int)Math.Round((B.ValueAt(x))));
                else output.Location = new Point(x, (int)Math.Round((A.ValueAt(x))));
            }
            return output;
        }
    }
    public class LineType
    {
        public static LineType Vertical = new LineType();
        public static LineType Cartesian = new LineType();
        public static LineType NonExistent = new LineType();
    }
    public class Intersection
    {
        public bool PointExists;
        public Point Location;
    }
    public class PointSelection
    {
        public static PictureBox Control;
        private bool Active = true;
        int num, selectedPoints = 0;
        Point[] Points;

        public static PointSelection Empty = new PointSelection(0);

        public PointSelection(int points)
        {
            this.num = points;
            this.Points = new Point[num];
        }

        public void Begin()
        {
            Control.MouseClick += SelectPoint;
        }
        public void End()
        {
            Active = false;
        }
        public delegate void PointsEvent(Point[] Points);
        public event PointsEvent PointsSelected, LastPointEvent;
        private void SelectPoint(object sender, MouseEventArgs e)
        {
            if (Active)
            {
                Points[selectedPoints] = e.Location;
                selectedPoints++;
                if (selectedPoints == num)
                {
                    Control.MouseClick -= SelectPoint;
                    Control.MouseMove -= LastPoint;
                    PointsSelected?.Invoke(Points);
                }

                if (selectedPoints == num - 1) Control.MouseMove += LastPoint;
            }
        }
        private void LastPoint(object sender, MouseEventArgs e)
        {
            if (Active && selectedPoints != num)
            {
                Points[num - 1] = e.Location;
                LastPointEvent?.Invoke(Points);
            }
        }
    }
}