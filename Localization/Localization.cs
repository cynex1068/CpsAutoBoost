using System.Globalization;

namespace WindowsCpsTool;

static class Localization
{
    public static readonly AppLanguage[] SupportedLanguages =
    {
        AppLanguage.System,
        AppLanguage.English,
        AppLanguage.French,
        AppLanguage.German,
        AppLanguage.Russian,
        AppLanguage.Ukrainian,
        AppLanguage.Chinese,
        AppLanguage.Japanese
    };

    public static AppLanguage DetectSystemLanguage()
    {
        return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant() switch
        {
            "fr" => AppLanguage.French,
            "de" => AppLanguage.German,
            "ru" => AppLanguage.Russian,
            "uk" => AppLanguage.Ukrainian,
            "zh" => AppLanguage.Chinese,
            "ja" => AppLanguage.Japanese,
            _ => AppLanguage.English
        };
    }

    public static string GetLanguageName(AppLanguage language)
    {
        return language switch
        {
            AppLanguage.System => "System",
            AppLanguage.English => "English",
            AppLanguage.French => "Français",
            AppLanguage.German => "Deutsch",
            AppLanguage.Russian => "Русский",
            AppLanguage.Ukrainian => "Українська",
            AppLanguage.Chinese => "中文",
            AppLanguage.Japanese => "日本語",
            _ => "English"
        };
    }

    public static UiText Get(AppLanguage language)
    {
        var actual = language == AppLanguage.System ? DetectSystemLanguage() : language;
        return actual switch
        {
            AppLanguage.French => new UiText("Langue", "Activer l'accélération automatique", "Les conditions de démarrage et les multiplicateurs sont personnalisables.", "Seuil gauche", "Seuil droit", "Multiplicateur gauche", "Multiplicateur droit", "Bouton", "CPS réel", "CPS total", "Gauche", "Droit", "Gauche : x ≥ {0}, y = {1}x\nDroit : x ≥ {2}, y = {3}x\nx compte uniquement les clics physiques.", "État : actif", "État : arrêté"),
            AppLanguage.German => new UiText("Sprache", "Automatische Beschleunigung aktivieren", "Startbedingungen und Multiplikatoren können angepasst werden.", "Linker Schwellenwert", "Rechter Schwellenwert", "Linker Multiplikator", "Rechter Multiplikator", "Taste", "Echte CPS", "Gesamte CPS", "Links", "Rechts", "Links: x ≥ {0}, y = {1}x\nRechts: x ≥ {2}, y = {3}x\nx zählt nur echte physische Klicks.", "Status: aktiv", "Status: angehalten"),
            AppLanguage.Russian => new UiText("Язык", "Включить автоматическое ускорение", "Условия запуска и множители можно настроить.", "Порог левой", "Порог правой", "Множитель левой", "Множитель правой", "Кнопка", "Реальный CPS", "Общий CPS", "Левая", "Правая", "Левая: x ≥ {0}, y = {1}x\nПравая: x ≥ {2}, y = {3}x\nx учитывает только физические клики.", "Состояние: включено", "Состояние: выключено"),
            AppLanguage.Ukrainian => new UiText("Мова", "Увімкнути автоматичне прискорення", "Умови запуску та множники можна налаштувати.", "Поріг лівої", "Поріг правої", "Множник лівої", "Множник правої", "Кнопка", "Справжній CPS", "Загальний CPS", "Ліва", "Права", "Ліва: x ≥ {0}, y = {1}x\nПрава: x ≥ {2}, y = {3}x\nx враховує лише фізичні кліки.", "Стан: увімкнено", "Стан: вимкнено"),
            AppLanguage.Chinese => new UiText("语言", "开启自动加速", "启动条件和左右键倍数都可以手动修改。", "左键启动条件", "右键启动条件", "左键倍数", "右键倍数", "按键", "真实 CPS", "总 CPS", "左键", "右键", "左键：x ≥ {0}，y = {1}x\n右键：x ≥ {2}，y = {3}x\nx 只统计真实物理点击。", "状态：运行中", "状态：已关闭"),
            AppLanguage.Japanese => new UiText("言語", "自動ブーストを有効化", "開始条件と左右クリック倍率を個別に設定できます。", "左クリック開始条件", "右クリック開始条件", "左クリック倍率", "右クリック倍率", "ボタン", "実 CPS", "合計 CPS", "左クリック", "右クリック", "左クリック：x ≥ {0}、y = {1}x\n右クリック：x ≥ {2}、y = {3}x\nx は物理クリックのみを数えます。", "状態：実行中", "状態：停止"),
            _ => new UiText("Language", "Enable automatic boost", "Start conditions and multipliers can be changed.", "Left start condition", "Right start condition", "Left multiplier", "Right multiplier", "Button", "Real CPS", "Total CPS", "Left", "Right", "Left: x ≥ {0}, y = {1}x\nRight: x ≥ {2}, y = {3}x\nx counts physical clicks only.", "Status: running", "Status: stopped")
        };
    }
}
