using System.Globalization;
using System.Windows.Forms;

namespace WindowsCpsTool;

internal sealed class MainForm : Form
{
    private readonly CheckBox enabledCheck = new();
    private readonly ComboBox languageBox = new();
    private readonly NumericUpDown leftThresholdBox = new();
    private readonly NumericUpDown rightThresholdBox = new();
    private readonly NumericUpDown leftMultiplierBox = new();
    private readonly NumericUpDown rightMultiplierBox = new();
    private readonly Label titleLabel = new();
    private readonly Label languageLabel = new();
    private readonly Label leftThresholdLabel = new();
    private readonly Label rightThresholdLabel = new();
    private readonly Label leftMultiplierLabel = new();
    private readonly Label rightMultiplierLabel = new();
    private readonly Label hintLabel = new();
    private readonly Label tableKeyLabel = new();
    private readonly Label tableRealLabel = new();
    private readonly Label tableTotalLabel = new();
    private readonly Label leftKeyLabel = new();
    private readonly Label rightKeyLabel = new();
    private readonly Label formulaLabel = new();
    private readonly Label statusLabel = new();
    private readonly Label leftRealValue = new();
    private readonly Label leftTotalValue = new();
    private readonly Label rightRealValue = new();
    private readonly Label rightTotalValue = new();
    private readonly System.Windows.Forms.Timer refreshTimer = new();
    private readonly ClickCounter leftCounter = new();
    private readonly ClickCounter rightCounter = new();
    private readonly object autoClickLock = new();
    private readonly System.Threading.Timer autoClickTimer;
    private GlobalMouseHook? mouseHook;
    private double leftBudget;
    private double rightBudget;
    private int automationEnabled;
    private int leftThreshold = 5;
    private int rightThreshold = 4;
    private int leftMultiplierPercent = 200;
    private int rightMultiplierPercent = 300;
    private AppLanguage selectedLanguage;

    public MainForm()
    {
        selectedLanguage = Localization.DetectSystemLanguage();
        Text = "CPS AUTO BOOST";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(650, 520);
        MinimumSize = new Size(650, 520);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Icon = SystemIcons.Application;

        BuildUi();
        ApplyLanguage();

        enabledCheck.CheckedChanged += (_, _) => SetAutomation(enabledCheck.Checked);
        languageBox.SelectedIndexChanged += (_, _) => ChangeLanguage();
        leftThresholdBox.ValueChanged += (_, _) =>
        {
            Volatile.Write(ref leftThreshold, (int)leftThresholdBox.Value);
            RefreshValues();
        };
        rightThresholdBox.ValueChanged += (_, _) =>
        {
            Volatile.Write(ref rightThreshold, (int)rightThresholdBox.Value);
            RefreshValues();
        };
        leftMultiplierBox.ValueChanged += (_, _) =>
        {
            Volatile.Write(ref leftMultiplierPercent, (int)(leftMultiplierBox.Value * 100));
            RefreshValues();
        };
        rightMultiplierBox.ValueChanged += (_, _) =>
        {
            Volatile.Write(ref rightMultiplierPercent, (int)(rightMultiplierBox.Value * 100));
            RefreshValues();
        };

        refreshTimer.Interval = 100;
        refreshTimer.Tick += (_, _) => RefreshValues();
        refreshTimer.Start();

        mouseHook = new GlobalMouseHook();
        mouseHook.MouseButtonDown += OnGlobalMouseButtonDown;
        mouseHook.Install();
        autoClickTimer = new System.Threading.Timer(_ => DriveAutoClicks(), null, 10, 10);
        FormClosed += (_, _) => mouseHook.Dispose();
        FormClosed += (_, _) => autoClickTimer.Dispose();
    }

    private void BuildUi()
    {
        BackColor = Color.FromArgb(245, 247, 250);
        titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        titleLabel.AutoSize = true;
        titleLabel.Location = new Point(24, 20);
        Controls.Add(titleLabel);

        languageLabel.AutoSize = true;
        languageLabel.Location = new Point(420, 29);
        Controls.Add(languageLabel);
        languageBox.DropDownStyle = ComboBoxStyle.DropDownList;
        languageBox.Location = new Point(490, 24);
        languageBox.Size = new Size(135, 28);
        foreach (var language in Localization.SupportedLanguages)
        {
            languageBox.Items.Add(new LanguageChoice(language, Localization.GetLanguageName(language)));
        }
        languageBox.SelectedIndex = 0;
        Controls.Add(languageBox);

        enabledCheck.AutoSize = true;
        enabledCheck.Font = new Font("Segoe UI", 10.5f);
        enabledCheck.Location = new Point(27, 70);
        Controls.Add(enabledCheck);

        hintLabel.AutoSize = true;
        hintLabel.ForeColor = Color.DimGray;
        hintLabel.Location = new Point(28, 102);
        Controls.Add(hintLabel);

        var settings = new TableLayoutPanel
        {
            Location = new Point(24, 132),
            Size = new Size(601, 116),
            ColumnCount = 4,
            RowCount = 2,
            BackColor = Color.White,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        };
        settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
        settings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
        AddSetting(settings, leftThresholdLabel, leftThresholdBox, 0, 5, 0, true);
        AddSetting(settings, leftMultiplierLabel, leftMultiplierBox, 0, 2, 2, false);
        AddSetting(settings, rightThresholdLabel, rightThresholdBox, 1, 4, 0, true);
        AddSetting(settings, rightMultiplierLabel, rightMultiplierBox, 1, 3, 2, false);
        Controls.Add(settings);

        var table = new TableLayoutPanel
        {
            Location = new Point(24, 264),
            Size = new Size(601, 130),
            ColumnCount = 3,
            RowCount = 3,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
            BackColor = Color.White
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        ConfigureTableLabel(tableKeyLabel, true);
        ConfigureTableLabel(tableRealLabel, true);
        ConfigureTableLabel(tableTotalLabel, true);
        ConfigureTableLabel(leftKeyLabel, true);
        ConfigureTableLabel(rightKeyLabel, true);
        table.Controls.Add(tableKeyLabel, 0, 0);
        table.Controls.Add(tableRealLabel, 1, 0);
        table.Controls.Add(tableTotalLabel, 2, 0);
        table.Controls.Add(leftKeyLabel, 0, 1);
        table.Controls.Add(rightKeyLabel, 0, 2);
        ConfigureValueLabel(leftRealValue);
        ConfigureValueLabel(leftTotalValue);
        ConfigureValueLabel(rightRealValue);
        ConfigureValueLabel(rightTotalValue);
        table.Controls.Add(leftRealValue, 1, 1);
        table.Controls.Add(leftTotalValue, 2, 1);
        table.Controls.Add(rightRealValue, 1, 2);
        table.Controls.Add(rightTotalValue, 2, 2);
        Controls.Add(table);

        formulaLabel.AutoSize = true;
        formulaLabel.Location = new Point(28, 410);
        formulaLabel.ForeColor = Color.FromArgb(70, 70, 70);
        Controls.Add(formulaLabel);

        statusLabel.AutoSize = true;
        statusLabel.Location = new Point(28, 475);
        statusLabel.ForeColor = Color.FromArgb(0, 100, 70);
        Controls.Add(statusLabel);
    }

    private static void AddSetting(TableLayoutPanel table, Label label, NumericUpDown box, int row, decimal value, int column, bool threshold)
    {
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleCenter;
        box.Minimum = 1;
        box.Maximum = threshold ? 100 : 20;
        box.DecimalPlaces = threshold ? 0 : 1;
        box.Increment = threshold ? 1 : 0.1m;
        box.Value = value;
        box.Width = 80;
        box.Anchor = AnchorStyles.None;
        table.Controls.Add(label, column, row);
        table.Controls.Add(box, column + 1, row);
    }

    private static void ConfigureTableLabel(Label label, bool bold)
    {
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.Font = new Font("Segoe UI", 9.5f, bold ? FontStyle.Bold : FontStyle.Regular);
    }

    private static void ConfigureValueLabel(Label label)
    {
        label.Dock = DockStyle.Fill;
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.Font = new Font("Consolas", 14, FontStyle.Bold);
        label.ForeColor = Color.FromArgb(25, 80, 170);
    }

    private void ChangeLanguage()
    {
        if (languageBox.SelectedItem is LanguageChoice choice)
        {
            selectedLanguage = choice.Language;
            ApplyLanguage();
        }
    }

    private void ApplyLanguage()
    {
        var text = Localization.Get(selectedLanguage);
        titleLabel.Text = "CPS AUTO BOOST";
        languageLabel.Text = text.Language;
        enabledCheck.Text = text.Enable;
        hintLabel.Text = text.Hint;
        leftThresholdLabel.Text = text.LeftThreshold;
        rightThresholdLabel.Text = text.RightThreshold;
        leftMultiplierLabel.Text = text.LeftMultiplier;
        rightMultiplierLabel.Text = text.RightMultiplier;
        tableKeyLabel.Text = text.Button;
        tableRealLabel.Text = text.RealCps;
        tableTotalLabel.Text = text.TotalCps;
        leftKeyLabel.Text = text.Left;
        rightKeyLabel.Text = text.Right;
        formulaLabel.Text = string.Format(text.Formula, leftThresholdBox.Value, leftMultiplierBox.Value, rightThresholdBox.Value, rightMultiplierBox.Value);
        statusLabel.Text = enabledCheck.Checked ? text.Running : text.Stopped;
    }

    private void SetAutomation(bool enabled)
    {
        Volatile.Write(ref automationEnabled, enabled ? 1 : 0);
        leftCounter.Clear();
        rightCounter.Clear();
        lock (autoClickLock)
        {
            leftBudget = 0;
            rightBudget = 0;
        }
        ApplyLanguage();
        RefreshValues();
    }

    private void OnGlobalMouseButtonDown(MouseButton button)
    {
        if (Volatile.Read(ref automationEnabled) == 0)
        {
            return;
        }
        if (button == MouseButton.Left)
        {
            leftCounter.AddClick();
        }
        else
        {
            rightCounter.AddClick();
        }
    }

    private void DriveAutoClicks()
    {
        if (Volatile.Read(ref automationEnabled) == 0)
        {
            return;
        }
        lock (autoClickLock)
        {
            const double elapsedSeconds = 0.01;
            var leftX = leftCounter.GetCountLastSecond();
            var rightX = rightCounter.GetCountLastSecond();
            var leftExtra = Math.Max(0, Volatile.Read(ref leftMultiplierPercent) / 100.0 - 1);
            var rightExtra = Math.Max(0, Volatile.Read(ref rightMultiplierPercent) / 100.0 - 1);
            leftBudget += GetExtraRate(leftX, Volatile.Read(ref leftThreshold), leftExtra) * elapsedSeconds;
            rightBudget += GetExtraRate(rightX, Volatile.Read(ref rightThreshold), rightExtra) * elapsedSeconds;
            var leftCount = (int)Math.Min(Math.Floor(leftBudget), 1000);
            var rightCount = (int)Math.Min(Math.Floor(rightBudget), 1000);
            if (leftCount > 0)
            {
                leftBudget -= leftCount;
                SendAutoClicks(MouseButton.Left, leftCount);
            }
            if (rightCount > 0)
            {
                rightBudget -= rightCount;
                SendAutoClicks(MouseButton.Right, rightCount);
            }
        }
    }

    private static double GetExtraRate(double x, int threshold, double extraMultiplier)
    {
        return x >= threshold ? x * extraMultiplier : 0;
    }

    private static void SendAutoClicks(MouseButton button, int count)
    {
        AutoClickGuard.Enter();
        try
        {
            MouseSimulator.Click(button, count);
        }
        finally
        {
            AutoClickGuard.Exit();
        }
    }

    private void RefreshValues()
    {
        var text = Localization.Get(selectedLanguage);
        var left = leftCounter.GetCountLastSecond();
        var right = rightCounter.GetCountLastSecond();
        var leftMultiplier = Volatile.Read(ref leftMultiplierPercent) / 100.0;
        var rightMultiplier = Volatile.Read(ref rightMultiplierPercent) / 100.0;
        leftRealValue.Text = left.ToString("0.0", CultureInfo.InvariantCulture);
        rightRealValue.Text = right.ToString("0.0", CultureInfo.InvariantCulture);
        leftTotalValue.Text = Calculate(left, Volatile.Read(ref leftThreshold), leftMultiplier).ToString("0.0", CultureInfo.InvariantCulture);
        rightTotalValue.Text = Calculate(right, Volatile.Read(ref rightThreshold), rightMultiplier).ToString("0.0", CultureInfo.InvariantCulture);
        formulaLabel.Text = string.Format(text.Formula, leftThresholdBox.Value, leftMultiplierBox.Value, rightThresholdBox.Value, rightMultiplierBox.Value);
    }

    private static double Calculate(double x, int threshold, double multiplier)
    {
        return x >= threshold ? x * multiplier : x;
    }
}
