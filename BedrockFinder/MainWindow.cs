﻿using BedrockFinder.Libraries;
using FastBitmapUtils;
using static BedrockSearch;

namespace BedrockFinder;

public partial class MainWindow : DHForm
{
    #region Window
    private void ShowInit()
    {
        Program.FormHandle = Handle;
        Instance();
    }
    private void ControlsInit()
    {
        Icon = SmallApp.Icon = Program.Resources.Get("Icon")?.GetContent<Icon>();

        ImportPatternPB.Image = Program.Resources.Get("ImportImage")?.GetContent<Image>();
        ToolTips.SetToolTip(ImportPatternPB, "Import Pattern");

        ExportPatternPB.Image = Program.Resources.Get("ExportImage")?.GetContent<Image>();
        ToolTips.SetToolTip(ExportPatternPB, "Export Pattern");

        ImportWorldPatternPB.Image = Program.Resources.Get("ImportWorldImage")?.GetContent<Image>();
        ToolTips.SetToolTip(ImportWorldPatternPB, "Import Pattern As World");

        ExportWorldPatternPB.Image = Program.Resources.Get("ExportWorldImage")?.GetContent<Image>();
        ToolTips.SetToolTip(ExportWorldPatternPB, "Export Pattern As World");

        ClearPatternPB.Image = Program.Resources.Get("ClearImage")?.GetContent<Image>();
        ToolTips.SetToolTip(ClearPatternPB, "Clear This Pattern Layer");

        RightTurnPB.Image = Program.Resources.Get("RightTurnImage")?.GetContent<Image>();
        ToolTips.SetToolTip(RightTurnPB, "Turn on Right");

        LeftTurnPB.Image = Program.Resources.Get("LeftTurnImage")?.GetContent<Image>();
        ToolTips.SetToolTip(LeftTurnPB, "Turn on Left");

        BackToStartPatternPB.Image = Program.Resources.Get("ZoomOutImage")?.GetContent<Image>();
        ToolTips.SetToolTip(BackToStartPatternPB, "Back to Start of Pattern");

        CopyFoundP.Image = Program.Resources.Get("CopyImage")?.GetContent<Image>();
        ToolTips.SetToolTip(CopyFoundP, "Copy All Found In Clipboard");

        ToolTips.SetToolTip(YLevelSelectorTrB, "Change Y Level For Pattern");

        DeviceSelectDHCB.Collection = new List<string>()
        {
            "CPU"
        };
        DeviceSelectDHCB.Collection.AddRange(Program.Devices.Select(z => "K -> " + z.Name));
        DeviceSelectDHCB.Text = "Device: ";
        DeviceSelectDHCB.ItemIndex = 0;
        DeviceSelectDHCB.IndexChange += DeviceChanged;

        VersionSelectDHCB.Collection = new List<string>()
        {
            "1.12",
            "1.13",
            "1.14",
            "1.15",
            "1.16",
            "1.17",
            "1.18",
        };
        VersionSelectDHCB.Text = "Version: ";
        VersionSelectDHCB.ItemIndex = 0;
        VersionSelectDHCB.IndexChange += VersionChanged;

        ContextSelectDHCB.Collection = new List<string>()
        {
            "Overworld",
            "Lower Nether",
            "Higher Nether",
        };
        ContextSelectDHCB.Text = "Context: ";
        ContextSelectDHCB.ItemIndex = 0;
        ContextSelectDHCB.IndexChange += ContextChanged;

        MainDisplayP.Round(25, false, true, true, true);
        CanvasSettingsP.Round(25, true, true, false, false);
        CanvasP.Round(25, true, true, false, true);
        CloseB.Round(15, true, true, false, false);
        MakeAsSmallAppB.Round(15, false, true, false, false);
        MainSettingsP.Round(25);
        SearchInfoP.Round(25);
        SearchManageP.Round(25);
        FoundP.Round(25);
        FoundListRTB.Round(20);
        SearchExportProgress.Round(20, false, true, false, false);
        SearchImportProgress.Round(20, true, false, false, false);
        SearchB.Round(20, false, false, true, false);
        SearchResetProgress.Round(20, false, false, false, true);
        DeviceSelectDHCB.Round(20);
        VersionSelectDHCB.Round(20);
        ContextSelectDHCB.Round(20);
        RangeP.Round(25);
    }
    private CanvasForm canvas = new CanvasForm() { TopLevel = false };
    public MainWindow()
    {
        InitializeComponent();
        CanvasP.Controls.Add(canvas);
        canvas.Show();
        canvas.Location = new Point(-30, -160);
        ControlsInit();
        ShowInit();       

        PenP.BackgroundImage = StoneFamilyBlock.DrawVectorPen(BlockType.Bedrock, canvas.Vector);
    }
    private void CloseB_Click(object sender, EventArgs e)
    {
        SafeSave();
        Environment.Exit(0);
    }
    private void MakeAsSmallAppB_Click(object sender, EventArgs e)
    {
        SmallApp.Visible = true;
        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;
    }
    private void SmallApp_Click(object sender, EventArgs e)
    {
        SmallApp.Visible = false;
        WindowState = FormWindowState.Normal;
        ShowInTaskbar = true;
        Activate();
        ShowInit();
    }
    #endregion
    #region Canvas
    public void UpdatePatternScore()
    {
        PatternScoreL.Text = "Score: " + Program.Pattern.CalculateScore();
        decimal predicted = Math.Round(Program.SearchRange.BlockRange * Program.Pattern.CalculateFindPercent(), 0, MidpointRounding.AwayFromZero);
        SearchPredictedCountL.Text = "Predicted Count: " + (predicted < 10000 ? predicted : "Much");
    }
    private void ImportPatternPB_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Pattern File|*.bfp;";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                canvas.UnDraw();
                Program.Pattern = ConfigManager.ImportPatternAsBFP(openFileDialog.FileName);
                canvas.OverDraw();
                UpdatePatternScore();
            }
        }        
    }
    private void ExportPatternPB_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Pattern File|*.bfp;";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = false; 
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(openFileDialog.FileName))
                    File.Create(openFileDialog.FileName).Dispose();
                ConfigManager.ExportPatternAsBFP(Program.Pattern, openFileDialog.FileName);
            }
        }
    }
    private void ExportWorldPatternPB_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(openFileDialog.SelectedPath))
                    Directory.CreateDirectory(openFileDialog.SelectedPath);
                ConfigManager.ExportPatternAsWorld(Program.Pattern, openFileDialog.SelectedPath);
            }
        }
    }
    private void ImportWorldPatternPB_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(openFileDialog.SelectedPath))
                    Directory.CreateDirectory(openFileDialog.SelectedPath);

                BedrockPattern? pattern = ConfigManager.ImportPatternAsWorld(openFileDialog.SelectedPath);
                if (pattern != null)
                {
                    canvas.UnDraw();
                    Program.Pattern = pattern;
                    canvas.OverDraw();
                    UpdatePatternScore();
                }
            }
        }
    }
    private void ClearPatternPB_Click(object sender, EventArgs e)
    {
        Program.Pattern[canvas.YLevel].blockList.ToList().ForEach(z =>
        {
            canvas.DrawBlock(z.x, 31 - z.z, BlockType.None);
            Program.Pattern[canvas.YLevel][z.x, z.z] = BlockType.None;
        });
        UpdatePatternScore();
    }
    private void PatternCurChecker_Tick(object sender, EventArgs e)
    {
        Point panel = new Point(
            Location.X + MainDisplayP.Location.X + CanvasP.Location.X,
            Location.Y + MainDisplayP.Location.Y + CanvasP.Location.Y
        );
        Point curDiff = new Point(Cursor.Position.X - panel.X, Cursor.Position.Y - panel.Y);
        if (curDiff.X > 0 && curDiff.X < 384 && curDiff.Y > 0 && curDiff.Y < 384)
        {
            Point pos = new Point(curDiff.X - canvas.Location.X - 30, 543 - (curDiff.Y - canvas.Location.Y));
            if (pos.X > 0 && pos.Y > 0 && pos.X < 543 && pos.Y < 543)
            {
                Point index = new Point(pos.X / 17, pos.Y / 17);
                (bool x, bool z) points = canvas.Vector.CurrentPoint;
                PatternCoordL.ForeColor = Color.Silver;
                PatternCoordL.Text = "C: " + (points.x ? "" : "-") + index.X + ", " + (points.z ? "" : "-") + index.Y;
                return;
            }
        }
        PatternCoordL.ForeColor = Color.Gray;
        PatternCoordL.Text = "C: NaN";
    }
    private void RightTurnPB_Click(object sender, EventArgs e)
    {
        canvas.Vector.Turn(-1);
        canvas.DrawPointers();
        canvas.OverDraw();
        canvas.Invalidate();
        PenP.Image = StoneFamilyBlock.DrawVectorPen(canvas.PenType, canvas.Vector);
    }
    private void LeftTurnPB_Click(object sender, EventArgs e)
    {
        canvas.Vector.Turn(1);
        canvas.DrawPointers();
        canvas.OverDraw();
        canvas.Invalidate();
        PenP.Image = StoneFamilyBlock.DrawVectorPen(canvas.PenType, canvas.Vector);
    }
    private void YLevelSelectorTrB_Scroll(object sender, EventArgs e)
    {
        YLevelL.Text = $"({YLevelSelectorTrB.Value})";
        canvas.UnDraw();
        canvas.YLevel = (byte)YLevelSelectorTrB.Value;
        canvas.OverDraw();
    }
    private void BackToStartPatternPB_Click(object sender, EventArgs e) => canvas.Location = new Point(-30, -160);
    private void PenP_Click(object sender, EventArgs e)
    {
        if (canvas.PenType == BlockType.Bedrock)
        {
            PenP.Image = StoneFamilyBlock.DrawVectorPen(BlockType.Stone, canvas.Vector);
            canvas.PenType = BlockType.Stone;
        }
        else
        {
            PenP.Image = StoneFamilyBlock.DrawVectorPen(BlockType.Bedrock, canvas.Vector);
            canvas.PenType = BlockType.Bedrock;
        }
    }
    #endregion
    #region Search
    private SearchStatus status = SearchStatus.PatternEdit;
    private object controlLock = new object();
    private void SearchB_Click(object sender, EventArgs e)
    {
        if (status == SearchStatus.PatternEdit || status == SearchStatus.Finish)
        {
            if(Program.Pattern.CalculateScore() < 30)
            {
                MessageBox.Show("not enough pattern score, minimum is 30");
                return;
            }
            FoundedCountL.Text = $"Found: 0";
            FoundListRTB.Text = "";
            if (Program.Search != null)
            {
                Program.Search.UpdateProgress -= UpdateProgress;
                Program.Search.Found -= FoundEvent;
            }
            Program.Search = new BedrockSearch(Program.Pattern, canvas.Vector, Program.SearchRange);
            Program.Search.UpdateProgress += UpdateProgress;
            Program.Search.Found += FoundEvent;
            status = SearchStatus.Search;
            Program.Search.Result = new List<Vec2i>();
            Program.Search.Start();
            SearchB.Text = "Stop Search";
        }
        else if (status == SearchStatus.Search)
        {
            Program.Search.Stop();
            status = SearchStatus.Pause;
            SearchB.Text = "Resume Search";
        }
        else if (status == SearchStatus.Pause)
        {
            if (Program.Search.CanStart && Program.Search.Resume())
            {
                status = SearchStatus.Search;
                SearchB.Text = "Stop Search";
            }
        }
        SearchStatusL.Text = $"Status: {statusStrings[status]}";
    }
    private void UpdateProgress(double progress)
    {
        lock (controlLock)
        {
            Invoke(() =>
            {
                if(progress == 100)
                {
                    status = SearchStatus.Finish;
                    SearchStatusL.Text = $"Status: {statusStrings[status]}";
                    SearchB.Text = "Start Search";
                    if (SearchElapsedTimeL.Text == "Elapsed Time:")
                        SearchElapsedTimeL.Text = $"Elapsed Time: 0s";
                }
                SearchProgressL.Text = $"Progress: {Math.Round(progress, 2, MidpointRounding.AwayFromZero)}%";
                SearchElapsedTimeL.Text = $"Elapsed Time: " + TimeSpanToString(Program.Search.Progress.ElapsedTime);
            });
        }
    }
    private void FoundEvent(Vec2i found)
    {
        lock (controlLock)
        {
            Invoke(() =>
            {
                FoundedCountL.Text = $"Found: " + Program.Search.Result.Count;
                FoundListRTB.Text += $"{Program.Search.Result.Count}. {found.X} {found.Z}\n";
            });
        }
    }
    Dictionary<SearchStatus, string> statusStrings = new Dictionary<SearchStatus, string>()
    {
        { SearchStatus.PatternEdit, "Pattern Editing" },
        { SearchStatus.Search, "Searching" },
        { SearchStatus.Finish, "Finished" },
        { SearchStatus.Pause, "Paused" },
    };

    private void SearchResetProgress_Click(object sender, EventArgs e)
    {
        if (status != SearchStatus.PatternEdit)
        {
            if (status == SearchStatus.Search)
                Program.Search.Stop();
            status = SearchStatus.PatternEdit;
            Program.Search.UpdateProgress -= UpdateProgress;
            Program.Search.Found -= FoundEvent;
            FoundedCountL.Text = $"Found: NaN";
            SearchB.Text = "Start Search";
            SearchProgressL.Text = $"Progress: NaN%";
            SearchElapsedTimeL.Text = $"Elapsed Time: NaN";
            FoundListRTB.Text = "";
        }
    }
    public static string TimeSpanToString(TimeSpan span) =>
    (span.Days > 365 ? span.Days / 365 + "y " : "")
    + (span.Days > 0 ? span.Days % 365 + "d " : "")
    + (span.Hours > 0 ? span.Hours + "h " : "")
    + (span.Minutes > 0 ? span.Minutes + "m " : "")
    + (span.Seconds > 0 ? span.Seconds + "s " : "");
    private void SearchExportProgress_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Pattern File|*.bfr;";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = false;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(openFileDialog.FileName))
                    File.Create(openFileDialog.FileName).Dispose();
                if(Program.Search == null)
                    Program.Search = new BedrockSearch(Program.Pattern, canvas.Vector, Program.SearchRange);
                ConfigManager.ExportSearchAsBFR(Program.Search, openFileDialog.FileName);
            }
        }
    }
    private void SearchImportProgress_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Pattern File|*.bfr;";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.CheckFileExists = false;
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(openFileDialog.FileName))
                    File.Create(openFileDialog.FileName).Dispose();
                canvas.UnDraw();
                Program.Search = ConfigManager.ImportSearchAsBFR(openFileDialog.FileName);
                Program.SearchRange = Program.Search.Range;
                Program.Pattern = Program.Search.Pattern;
                canvas.Vector = Program.Search.Vector;
                canvas.OverDraw();
                FoundListRTB.Text = string.Join('\n', Program.Search.Result.Select((z, i) => $"{i+1}. {z.X} {z.X}"));
                FoundedCountL.Text = $"Found: {Program.Search.Result.Count}";
                XAtTB.Text = Program.SearchRange.Start.X.ToString();
                ZAtTB.Text = Program.SearchRange.Start.Z.ToString();
                XToTB.Text = Program.SearchRange.End.X.ToString();
                ZToTB.Text = Program.SearchRange.End.Z.ToString();
                SearchElapsedTimeL.Text = $"Elapsed Time: {TimeSpanToString(Program.Search.Progress.ElapsedTime)}";
                SearchProgressL.Text = $"Progress: {Math.Round(Program.Search.Progress.GetPercent(), 2, MidpointRounding.AwayFromZero)}%";
                RangeSizeL.Text = $"Size {Program.SearchRange.XSize}x{Program.SearchRange.ZSize}";
            }
        }
    }
    private void CopyFoundP_Click(object sender, EventArgs e)
    {
        if (FoundListRTB.Text != "")
            Clipboard.SetText(FoundListRTB.Text);
    }
    #endregion
    #region Settings
    private void VersionChanged(int index)
    {

    }
    private void ContextChanged(int index)
    {

    }
    private void DeviceChanged(int index)
    {

    }
    #endregion
    #region Range
    private void UpdateRange(object sender, EventArgs e)
    {
        TextBox dSender = (TextBox)sender;
        if (ValidateTextCoord(dSender.Text))
        {
            dSender.ForeColor = Color.Silver;
            if (ValidateTextCoord(XAtTB.Text) && ValidateTextCoord(ZAtTB.Text) && ValidateTextCoord(XToTB.Text) && ValidateTextCoord(ZToTB.Text))
            {
                Program.SearchRange = new SearchRange(new Vec2l(int.Parse(XAtTB.Text), int.Parse(ZAtTB.Text)), new Vec2l(int.Parse(XToTB.Text), int.Parse(ZToTB.Text)));
                RangeSizeL.Text = $"Size {Program.SearchRange.XSize}x{Program.SearchRange.ZSize}";
                UpdatePatternScore();
            }
            return;
        }
        dSender.ForeColor = Color.Red;
    }
    private bool ValidateTextCoord(string text) => int.TryParse(text, out int num) && num >= -30000000 && num <= 30000000; 
    #endregion
    private void SafeSave()
    {

    }
}