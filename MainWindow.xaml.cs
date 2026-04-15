using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace LotusPecasApkInstaller;

public partial class MainWindow : Window
{
    private readonly string _adbPath;
    private string _lang = "tr";

    private static readonly Dictionary<string, Dictionary<string, string>> Strings = new()
    {
        ["tr"] = new()
        {
            ["subtitle"] = "Android cihazlara APK yükle",
            ["device"] = "Cihaz:",
            ["refresh"] = "🔄 Yenile",
            ["show"] = "🖥 Göster",
            ["scrcpyMissing"] = "scrcpy.exe bulunamadı",
            ["launchingScrcpy"] = "Ekran açılıyor: {0}",
            ["ip"] = "IP:",
            ["connect"] = "🔌 Bağlan",
            ["disconnect"] = "⏏ Kes",
            ["apk"] = "APK:",
            ["browse"] = "📂 Gözat",
            ["install"] = "⬇  APK Yükle",
            ["ready"] = "Hazır",
            ["noApk"] = "APK dosyası seçilmedi...",
            ["ipTip"] = "Örn: 192.168.1.42 veya 192.168.1.42:5555",
            ["searching"] = "Cihazlar aranıyor...",
            ["adbMissing"] = "adb.exe yok",
            ["devicesFound"] = "{0} cihaz bulundu",
            ["noDevices"] = "Cihaz bulunamadı",
            ["apkSelected"] = "APK seçildi",
            ["selectDevice"] = "Lütfen bir cihaz seçin.",
            ["selectApk"] = "Lütfen geçerli bir APK seçin.",
            ["warn"] = "Uyarı",
            ["installing"] = "Yükleniyor: {0}",
            ["success"] = "✓ Yükleme başarılı",
            ["successMsg"] = "APK başarıyla yüklendi!",
            ["successTitle"] = "Başarılı",
            ["failed"] = "✗ Yükleme başarısız",
            ["failedMsg"] = "Yükleme başarısız. Log'u kontrol edin.",
            ["error"] = "Hata",
            ["enterIp"] = "IP adresi girin.",
            ["connecting"] = "Bağlanılıyor: {0}",
            ["connected"] = "✓ Bağlandı: {0}",
            ["connectFailed"] = "✗ Bağlantı başarısız",
            ["apkFilter"] = "APK Dosyaları (*.apk)|*.apk|Tüm Dosyalar (*.*)|*.*",
            ["selectApkTitle"] = "APK Seç",
            ["pairTitle"] = "🔐 Kablosuz Eşleştirme (Android 11+)",
            ["pairHost"] = "① Eşleştirme IP:Port",
            ["pairCode"] = "Kod",
            ["pairConnect"] = "② Bağlantı IP:Port",
            ["pairButton"] = "🔗 Eşleştir ve Bağlan",
            ["pairHint"] = "Telefonda: Geliştirici seçenekleri → Kablosuz hata ayıklama.\n① Eşleştirme portu: 'Eşleştirme kodu ile cihaz eşleştir' ekranında görünür (kod ile birlikte).\n② Bağlantı portu: ana Kablosuz hata ayıklama ekranında 'IP adresi ve port' altında görünür — pair portundan farklıdır.",
            ["pairHostTip"] = "Örn: 192.168.1.20:37251",
            ["pairCodeTip"] = "6 haneli eşleştirme kodu",
            ["pairConnectTip"] = "Örn: 192.168.1.20:41235 (pair portundan FARKLI)",
            ["pairMissing"] = "Eşleştirme IP:Port ve kodu girin.",
            ["pairBadHost"] = "Eşleştirme adresi 'IP:Port' formatında olmalı.",
            ["pairing"] = "Eşleştiriliyor: {0}",
            ["paired"] = "✓ Eşleştirme başarılı",
            ["pairFailed"] = "✗ Eşleştirme başarısız",
        },
        ["de"] = new()
        {
            ["subtitle"] = "APK auf Android-Geräten installieren",
            ["device"] = "Gerät:",
            ["refresh"] = "🔄 Aktualisieren",
            ["show"] = "🖥 Anzeigen",
            ["scrcpyMissing"] = "scrcpy.exe nicht gefunden",
            ["launchingScrcpy"] = "Bildschirm öffnen: {0}",
            ["ip"] = "IP:",
            ["connect"] = "🔌 Verbinden",
            ["disconnect"] = "⏏ Trennen",
            ["apk"] = "APK:",
            ["browse"] = "📂 Durchsuchen",
            ["install"] = "⬇  APK installieren",
            ["ready"] = "Bereit",
            ["noApk"] = "Keine APK-Datei ausgewählt...",
            ["ipTip"] = "Bsp.: 192.168.1.42 oder 192.168.1.42:5555",
            ["searching"] = "Geräte werden gesucht...",
            ["adbMissing"] = "adb.exe fehlt",
            ["devicesFound"] = "{0} Gerät(e) gefunden",
            ["noDevices"] = "Kein Gerät gefunden",
            ["apkSelected"] = "APK ausgewählt",
            ["selectDevice"] = "Bitte ein Gerät auswählen.",
            ["selectApk"] = "Bitte eine gültige APK wählen.",
            ["warn"] = "Warnung",
            ["installing"] = "Installiere: {0}",
            ["success"] = "✓ Installation erfolgreich",
            ["successMsg"] = "APK erfolgreich installiert!",
            ["successTitle"] = "Erfolg",
            ["failed"] = "✗ Installation fehlgeschlagen",
            ["failedMsg"] = "Installation fehlgeschlagen. Log prüfen.",
            ["error"] = "Fehler",
            ["enterIp"] = "IP-Adresse eingeben.",
            ["connecting"] = "Verbinde: {0}",
            ["connected"] = "✓ Verbunden: {0}",
            ["connectFailed"] = "✗ Verbindung fehlgeschlagen",
            ["apkFilter"] = "APK-Dateien (*.apk)|*.apk|Alle Dateien (*.*)|*.*",
            ["selectApkTitle"] = "APK auswählen",
            ["pairTitle"] = "🔐 Drahtlose Kopplung (Android 11+)",
            ["pairHost"] = "① Pairing IP:Port",
            ["pairCode"] = "Code",
            ["pairConnect"] = "② Verbindungs-IP:Port",
            ["pairButton"] = "🔗 Koppeln und verbinden",
            ["pairHint"] = "Am Telefon: Entwickleroptionen → Drahtloses Debugging.\n① Pairing-Port: im Bildschirm 'Gerät mit Kopplungscode koppeln' (mit Code).\n② Verbindungs-Port: im Hauptbildschirm 'Drahtloses Debugging' unter 'IP-Adresse & Port' — unterscheidet sich vom Pairing-Port.",
            ["pairHostTip"] = "Bsp.: 192.168.1.20:37251",
            ["pairCodeTip"] = "6-stelliger Kopplungscode",
            ["pairConnectTip"] = "Bsp.: 192.168.1.20:41235 (ANDERS als Pairing-Port)",
            ["pairMissing"] = "Pairing IP:Port und Code eingeben.",
            ["pairBadHost"] = "Pairing-Adresse muss im Format 'IP:Port' sein.",
            ["pairing"] = "Koppeln: {0}",
            ["paired"] = "✓ Kopplung erfolgreich",
            ["pairFailed"] = "✗ Kopplung fehlgeschlagen",
        },
        ["fr"] = new()
        {
            ["subtitle"] = "Installer des APK sur appareils Android",
            ["device"] = "Appareil :",
            ["refresh"] = "🔄 Actualiser",
            ["show"] = "🖥 Afficher",
            ["scrcpyMissing"] = "scrcpy.exe introuvable",
            ["launchingScrcpy"] = "Ouverture de l'écran : {0}",
            ["ip"] = "IP :",
            ["connect"] = "🔌 Connecter",
            ["disconnect"] = "⏏ Déconnecter",
            ["apk"] = "APK :",
            ["browse"] = "📂 Parcourir",
            ["install"] = "⬇  Installer l'APK",
            ["ready"] = "Prêt",
            ["noApk"] = "Aucun fichier APK sélectionné...",
            ["ipTip"] = "Ex : 192.168.1.42 ou 192.168.1.42:5555",
            ["searching"] = "Recherche d'appareils...",
            ["adbMissing"] = "adb.exe introuvable",
            ["devicesFound"] = "{0} appareil(s) trouvé(s)",
            ["noDevices"] = "Aucun appareil trouvé",
            ["apkSelected"] = "APK sélectionné",
            ["selectDevice"] = "Veuillez sélectionner un appareil.",
            ["selectApk"] = "Veuillez choisir une APK valide.",
            ["warn"] = "Avertissement",
            ["installing"] = "Installation : {0}",
            ["success"] = "✓ Installation réussie",
            ["successMsg"] = "APK installée avec succès !",
            ["successTitle"] = "Succès",
            ["failed"] = "✗ Échec de l'installation",
            ["failedMsg"] = "Installation échouée. Vérifiez le journal.",
            ["error"] = "Erreur",
            ["enterIp"] = "Saisir une adresse IP.",
            ["connecting"] = "Connexion : {0}",
            ["connected"] = "✓ Connecté : {0}",
            ["connectFailed"] = "✗ Échec de la connexion",
            ["apkFilter"] = "Fichiers APK (*.apk)|*.apk|Tous les fichiers (*.*)|*.*",
            ["selectApkTitle"] = "Choisir APK",
            ["pairTitle"] = "🔐 Appairage sans fil (Android 11+)",
            ["pairHost"] = "① IP:Port d'appairage",
            ["pairCode"] = "Code",
            ["pairConnect"] = "② IP:Port de connexion",
            ["pairButton"] = "🔗 Appairer et connecter",
            ["pairHint"] = "Sur le téléphone : Options développeur → Débogage sans fil.\n① Port d'appairage : écran 'Appairer l'appareil avec un code' (avec le code).\n② Port de connexion : écran principal 'Débogage sans fil' sous 'Adresse IP et port' — différent du port d'appairage.",
            ["pairHostTip"] = "Ex : 192.168.1.20:37251",
            ["pairCodeTip"] = "Code d'appairage à 6 chiffres",
            ["pairConnectTip"] = "Ex : 192.168.1.20:41235 (DIFFÉRENT du port d'appairage)",
            ["pairMissing"] = "Saisir IP:Port d'appairage et code.",
            ["pairBadHost"] = "L'adresse d'appairage doit être au format 'IP:Port'.",
            ["pairing"] = "Appairage : {0}",
            ["paired"] = "✓ Appairage réussi",
            ["pairFailed"] = "✗ Échec de l'appairage",
        },
    };

    private string T(string key) => Strings[_lang].TryGetValue(key, out var v) ? v : key;

    public MainWindow()
    {
        InitializeComponent();
        _adbPath = DetectAdb();
        Loaded += async (_, _) =>
        {
            ApplyLanguage();
            Log($"ADB: {_adbPath}");
            await LoadDevicesAsync();
        };
    }

    private void Language_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button b && b.Tag is string code)
        {
            _lang = code;
            ApplyLanguage();
        }
    }

    private void ApplyLanguage()
    {
        SubtitleText.Text = T("subtitle");
        DeviceLabel.Text = T("device");
        RefreshButton.Content = T("refresh");
        ShowButton.Content = T("show");
        IpLabel.Text = T("ip");
        IpTextBox.ToolTip = T("ipTip");
        ConnectButton.Content = T("connect");
        DisconnectButton.Content = T("disconnect");
        PairTitleText.Text = T("pairTitle");
        PairHostLabel.Text = T("pairHost");
        PairCodeLabel.Text = T("pairCode");
        PairConnectLabel.Text = T("pairConnect");
        PairButton.Content = T("pairButton");
        PairHintText.Text = T("pairHint");
        PairHostTextBox.ToolTip = T("pairHostTip");
        PairCodeTextBox.ToolTip = T("pairCodeTip");
        PairConnectTextBox.ToolTip = T("pairConnectTip");
        ApkLabel.Text = T("apk");
        BrowseButton.Content = T("browse");
        InstallButton.Content = T("install");
        if (ApkPathTextBox.IsReadOnly && !System.IO.File.Exists(ApkPathTextBox.Text))
            ApkPathTextBox.Text = T("noApk");
        StatusText.Text = T("ready");
    }

    private static string DetectAdb()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var candidates = new[]
        {
            Path.Combine(baseDir, "adb.exe"),
            Path.GetFullPath(Path.Combine(baseDir, "..", "adb.exe")),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "adb.exe")),
        };
        foreach (var c in candidates)
            if (File.Exists(c)) return c;

        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? "";
        foreach (var dir in pathEnv.Split(Path.PathSeparator))
        {
            try
            {
                var p = Path.Combine(dir.Trim(), "adb.exe");
                if (File.Exists(p)) return p;
            }
            catch { }
        }

        var sdk = Environment.GetEnvironmentVariable("ANDROID_HOME")
                  ?? Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
        if (!string.IsNullOrEmpty(sdk))
        {
            var p = Path.Combine(sdk, "platform-tools", "adb.exe");
            if (File.Exists(p)) return p;
        }

        return Path.Combine(baseDir, "adb.exe");
    }

    private async void RefreshDevices_Click(object sender, RoutedEventArgs e) => await LoadDevicesAsync();

    private void ShowScreen_Click(object sender, RoutedEventArgs e)
    {
        if (DeviceComboBox.SelectedItem is not string device)
        {
            CustomMessageBox.Show(this, T("selectDevice"), T("warn"), CustomBoxIcon.Warning);
            return;
        }

        var scrcpyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scrcpy.exe");
        if (!File.Exists(scrcpyPath))
        {
            CustomMessageBox.Show(this, T("scrcpyMissing"), T("error"), CustomBoxIcon.Error);
            return;
        }

        try
        {
            SetStatus(string.Format(T("launchingScrcpy"), device));
            Log($"\n>>> scrcpy -s {device}");
            var psi = new ProcessStartInfo
            {
                FileName = scrcpyPath,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            psi.ArgumentList.Add("-s");
            psi.ArgumentList.Add(device);
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Log($"ERROR: {ex.Message}");
            CustomMessageBox.Show(this, ex.Message, T("error"), CustomBoxIcon.Error);
        }
    }

    private async void ConnectIp_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(ip))
        {
            CustomMessageBox.Show(this, T("enterIp"), T("warn"), CustomBoxIcon.Warning);
            return;
        }
        if (!ip.Contains(':')) ip += ":5555";

        SetStatus(string.Format(T("connecting"), ip));
        Log($"\n>>> adb connect {ip}");
        var result = await RunAdbAsync("connect", ip);
        Log(result);

        await LoadDevicesAsync();

        if (result.Contains("connected", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var item in DeviceComboBox.Items)
                if (item?.ToString() == ip) { DeviceComboBox.SelectedItem = item; break; }
            SetStatus(string.Format(T("connected"), ip));
        }
        else
        {
            SetStatus(T("connectFailed"));
        }
    }

    private async void PairDevice_Click(object sender, RoutedEventArgs e)
    {
        var pairAddr = PairHostTextBox.Text?.Trim();
        var code = PairCodeTextBox.Text?.Trim();

        if (string.IsNullOrWhiteSpace(pairAddr) || string.IsNullOrWhiteSpace(code))
        {
            CustomMessageBox.Show(this, T("pairMissing"), T("warn"), CustomBoxIcon.Warning);
            return;
        }
        if (!pairAddr.Contains(':'))
        {
            CustomMessageBox.Show(this, T("pairBadHost"), T("warn"), CustomBoxIcon.Warning);
            return;
        }

        PairButton.IsEnabled = false;
        try
        {
            SetStatus(string.Format(T("pairing"), pairAddr));
            Log($"\n>>> adb pair {pairAddr} {new string('*', code.Length)}");
            var pairResult = await RunAdbAsync("pair", pairAddr, code);
            Log(pairResult);

            var success = pairResult.Contains("Successfully paired", StringComparison.OrdinalIgnoreCase)
                          || pairResult.Contains("paired to", StringComparison.OrdinalIgnoreCase);

            if (!success)
            {
                SetStatus(T("pairFailed"));
                CustomMessageBox.Show(this, T("pairFailed"), T("error"), CustomBoxIcon.Error);
                return;
            }

            SetStatus(T("paired"));
            PairCodeTextBox.Clear();

            // Auto-connect: use dedicated PairConnectTextBox, fall back to IpTextBox, else derive from pair host
            var connectTarget = PairConnectTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(connectTarget))
                connectTarget = IpTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(connectTarget))
            {
                var host = pairAddr.Split(':')[0];
                connectTarget = host + ":5555";
            }
            else if (!connectTarget.Contains(':'))
            {
                connectTarget += ":5555";
            }
            IpTextBox.Text = connectTarget;

            SetStatus(string.Format(T("connecting"), connectTarget));
            Log($"\n>>> adb connect {connectTarget}");
            var connResult = await RunAdbAsync("connect", connectTarget);
            Log(connResult);

            await LoadDevicesAsync();

            if (connResult.Contains("connected", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in DeviceComboBox.Items)
                    if (item?.ToString() == connectTarget) { DeviceComboBox.SelectedItem = item; break; }
                SetStatus(string.Format(T("connected"), connectTarget));
            }
            else
            {
                SetStatus(T("connectFailed"));
            }
        }
        finally
        {
            PairButton.IsEnabled = true;
        }
    }

    private async void DisconnectIp_Click(object sender, RoutedEventArgs e)
    {
        var ip = IpTextBox.Text?.Trim();
        string result;
        if (string.IsNullOrWhiteSpace(ip))
        {
            Log("\n>>> adb disconnect");
            result = await RunAdbAsync("disconnect");
        }
        else
        {
            var target = ip.Contains(':') ? ip : ip + ":5555";
            Log($"\n>>> adb disconnect {target}");
            result = await RunAdbAsync("disconnect", target);
        }
        Log(result);
        await LoadDevicesAsync();
    }

    private async Task LoadDevicesAsync()
    {
        SetStatus(T("searching"));
        DeviceComboBox.Items.Clear();

        if (!File.Exists(_adbPath))
        {
            Log($"ERROR: adb.exe not found: {_adbPath}");
            SetStatus(T("adbMissing"));
            return;
        }

        var output = await RunAdbAsync("devices");
        Log(output);

        foreach (var line in output.Split('\n'))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("List of"))
                continue;
            var parts = trimmed.Split('\t');
            if (parts.Length >= 2 && parts[1].Trim() == "device")
                DeviceComboBox.Items.Add(parts[0]);
        }

        if (DeviceComboBox.Items.Count > 0)
        {
            DeviceComboBox.SelectedIndex = 0;
            SetStatus(string.Format(T("devicesFound"), DeviceComboBox.Items.Count));
        }
        else
        {
            SetStatus(T("noDevices"));
        }
    }

    private void BrowseApk_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = T("apkFilter"),
            Title = T("selectApkTitle")
        };
        if (dlg.ShowDialog() == true)
        {
            ApkPathTextBox.Text = dlg.FileName;
            SetStatus(T("apkSelected"));
        }
    }

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        if (DeviceComboBox.SelectedItem is not string device)
        {
            CustomMessageBox.Show(this, T("selectDevice"), T("warn"), CustomBoxIcon.Warning);
            return;
        }

        var apk = ApkPathTextBox.Text;
        if (string.IsNullOrWhiteSpace(apk) || !File.Exists(apk))
        {
            CustomMessageBox.Show(this, T("selectApk"), T("warn"), CustomBoxIcon.Warning);
            return;
        }

        InstallButton.IsEnabled = false;
        SetStatus(string.Format(T("installing"), Path.GetFileName(apk)));
        Log($"\n>>> adb -s {device} install -r \"{apk}\"");

        var result = await RunAdbAsync("-s", device, "install", "-r", apk);
        Log(result);

        if (result.Contains("Success", StringComparison.OrdinalIgnoreCase))
        {
            SetStatus(T("success"));
            CustomMessageBox.Show(this, T("successMsg"), T("successTitle"), CustomBoxIcon.Success);
        }
        else
        {
            SetStatus(T("failed"));
            CustomMessageBox.Show(this, T("failedMsg"), T("error"), CustomBoxIcon.Error);
        }

        InstallButton.IsEnabled = true;
    }

    private Task<string> RunAdbAsync(params string[] args)
    {
        return Task.Run(() =>
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = _adbPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                foreach (var a in args) psi.ArgumentList.Add(a);
                using var p = Process.Start(psi)!;
                var stdout = p.StandardOutput.ReadToEnd();
                var stderr = p.StandardError.ReadToEnd();
                p.WaitForExit();
                return (stdout + stderr).Trim();
            }
            catch (Exception ex)
            {
                return $"HATA: {ex.Message}";
            }
        });
    }

    private void Log(string text)
    {
        Dispatcher.Invoke(() =>
        {
            LogText.Text += text + "\n";
            LogScroller.ScrollToEnd();
        });
    }

    private void SetStatus(string text)
    {
        Dispatcher.Invoke(() => StatusText.Text = text);
    }
}
