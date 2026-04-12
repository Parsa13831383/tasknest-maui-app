using System.Globalization;

namespace TaskNest.Services;

public sealed class LocalizationService
{
    private const string LanguagePreferenceKey = "app.language";
    public static LocalizationService Instance { get; } = new();

    private readonly Dictionary<string, Dictionary<string, string>> _translations =
        new(StringComparer.Ordinal)
        {
            ["fr"] = new(StringComparer.Ordinal)
            {
                ["Dashboard"] = "Tableau de bord",
                ["Tasks"] = "Taches",
                ["Categories"] = "Categories",
                ["Settings"] = "Parametres",
                ["Profile"] = "Profil",
                ["WELCOME BACK"] = "BON RETOUR",
                ["TaskNest Dashboard"] = "Tableau de bord TaskNest",
                ["Stay on top of your tasks and progress."] = "Gardez le controle de vos taches et de vos progres.",
                ["Today"] = "Aujourd'hui",
                ["Completed"] = "Terminees",
                ["This week"] = "Cette semaine",
                ["Quick Actions"] = "Actions rapides",
                ["+ New Task"] = "+ Nouvelle tache",
                ["View Tasks"] = "Voir les taches",
                ["Today's Focus"] = "Priorites du jour",
                ["Manage your workload and stay organised."] = "Gerez votre charge de travail et restez organise.",
                ["+ Create Task"] = "+ Creer une tache",
                ["Filter Tasks"] = "Filtrer les taches",
                ["Search & Filters"] = "Recherche et filtres",
                ["Search tasks..."] = "Rechercher des taches...",
                ["Category"] = "Categorie",
                ["Your Tasks"] = "Vos taches",
                ["Task Details"] = "Details de la tache",
                ["Review task information, progress, and next actions."] = "Consultez les informations de la tache, les progres et les prochaines actions.",
                ["Description"] = "Description",
                ["Reflection"] = "Reflexion",
                ["Due Date"] = "Date d'echeance",
                ["Back to Tasks"] = "Retour aux taches",
                ["Edit Task"] = "Modifier la tache",
                ["Delete"] = "Supprimer",
                ["Edit Task"] = "Modifier la tache",
                ["Update task details, deadline, and progress."] = "Mettez a jour les details, l'echeance et les progres de la tache.",
                ["Task Information"] = "Informations de la tache",
                ["Task Title"] = "Titre de la tache",
                ["Enter task title"] = "Saisissez le titre de la tache",
                ["Enter task description"] = "Saisissez la description de la tache",
                ["Status & Deadline"] = "Statut et echeance",
                ["Type a category"] = "Saisir une categorie",
                ["Task Color"] = "Couleur de la tache",
                ["Task Reflection"] = "Reflexion sur la tache",
                ["Write quick notes about progress, blockers, or what you learned."] = "Ecrivez des notes rapides sur les progres, les blocages ou ce que vous avez appris.",
                ["Add your reflection here..."] = "Ajoutez votre reflexion ici...",
                ["Cancel"] = "Annuler",
                ["Save Changes"] = "Enregistrer",
                ["Organise your tasks into clear areas of focus."] = "Organisez vos taches en domaines de concentration clairs.",
                ["+ Add Category"] = "+ Ajouter une categorie",
                ["Manage Categories"] = "Gerer les categories",
                ["Total"] = "Total",
                ["Most Active"] = "La plus active",
                ["Areas of Focus"] = "Domaines de concentration",
                ["Edit"] = "Modifier",
                ["No reflection yet."] = "Pas encore de reflexion.",
                ["Settings"] = "Parametres",
                ["Manage your app preferences and account experience."] = "Gerez vos preferences et votre experience de compte.",
                ["Preferences"] = "Preferences",
                ["Enable Notifications"] = "Activer les notifications",
                ["Receive reminders about task deadlines."] = "Recevez des rappels sur les echeances des taches.",
                ["Dark Mode"] = "Mode sombre",
                ["Toggle a darker appearance for the app."] = "Activer une apparence plus sombre pour l'application.",
                ["Appearance & Language"] = "Apparence et langue",
                ["Theme"] = "Theme",
                ["Select theme"] = "Selectionner un theme",
                ["Language"] = "Langue",
                ["Select language"] = "Selectionner une langue",
                ["Productivity"] = "Productivite",
                ["Reminder Frequency"] = "Frequence des rappels",
                ["Choose reminder frequency"] = "Choisir la frequence des rappels",
                ["Save Settings"] = "Enregistrer les parametres",
                ["Reset"] = "Reinitialiser",
                ["Account"] = "Compte",
                ["Sign out of your account on this device."] = "Deconnectez-vous de votre compte sur cet appareil.",
                ["Logout"] = "Deconnexion",
                ["Welcome back"] = "Bon retour",
                ["Login to TaskNest"] = "Connexion a TaskNest",
                ["Access your tasks, categories, and productivity dashboard."] = "Accedez a vos taches, categories et tableau de bord.",
                ["Account Login"] = "Connexion au compte",
                ["Email Address"] = "Adresse e-mail",
                ["Enter your email"] = "Saisissez votre e-mail",
                ["Password"] = "Mot de passe",
                ["Enter your password"] = "Saisissez votre mot de passe",
                ["Remember me"] = "Se souvenir de moi",
                ["Forgot password?"] = "Mot de passe oublie ?",
                ["Login"] = "Connexion",
                ["Create an account"] = "Creer un compte",
                ["Demo login page for TaskNest mobile coursework project."] = "Page de connexion de demonstration pour le projet mobile TaskNest.",
                ["Create account"] = "Creer un compte",
                ["Join TaskNest and manage your productivity."] = "Rejoignez TaskNest et gerez votre productivite.",
                ["Register"] = "Inscription",
                ["Full Name"] = "Nom complet",
                ["Enter your name"] = "Saisissez votre nom",
                ["Email"] = "E-mail",
                ["Create password"] = "Creer un mot de passe",
                ["Confirm Password"] = "Confirmer le mot de passe",
                ["Confirm password"] = "Confirmer le mot de passe",
                ["Create Account"] = "Creer le compte",
                ["Already have an account? Login"] = "Vous avez deja un compte ? Connectez-vous",
                ["English"] = "Anglais",
                ["French"] = "Francais",
                ["Spanish"] = "Espagnol",
                ["Light"] = "Clair",
                ["Dark"] = "Sombre",
                ["System Default"] = "Systeme",
                ["Daily"] = "Quotidien",
                ["Weekly"] = "Hebdomadaire",
                ["Only for deadlines"] = "Uniquement pour les echeances",
                ["Settings saved successfully."] = "Parametres enregistres avec succes.",
                ["You have been signed out."] = "Vous avez ete deconnecte.",
                ["OK"] = "OK"
            },
            ["es"] = new(StringComparer.Ordinal)
            {
                ["Dashboard"] = "Panel",
                ["Tasks"] = "Tareas",
                ["Categories"] = "Categorias",
                ["Settings"] = "Configuracion",
                ["Profile"] = "Perfil",
                ["WELCOME BACK"] = "BIENVENIDO",
                ["TaskNest Dashboard"] = "Panel de TaskNest",
                ["Stay on top of your tasks and progress."] = "Mantente al dia con tus tareas y progreso.",
                ["Today"] = "Hoy",
                ["Completed"] = "Completadas",
                ["This week"] = "Esta semana",
                ["Quick Actions"] = "Acciones rapidas",
                ["+ New Task"] = "+ Nueva tarea",
                ["View Tasks"] = "Ver tareas",
                ["Today's Focus"] = "Enfoque de hoy",
                ["Manage your workload and stay organised."] = "Gestiona tu carga de trabajo y mantenla organizada.",
                ["+ Create Task"] = "+ Crear tarea",
                ["Filter Tasks"] = "Filtrar tareas",
                ["Search & Filters"] = "Busqueda y filtros",
                ["Search tasks..."] = "Buscar tareas...",
                ["Category"] = "Categoria",
                ["Your Tasks"] = "Tus tareas",
                ["Task Details"] = "Detalles de la tarea",
                ["Review task information, progress, and next actions."] = "Revisa la informacion de la tarea, el progreso y las siguientes acciones.",
                ["Description"] = "Descripcion",
                ["Reflection"] = "Reflexion",
                ["Due Date"] = "Fecha limite",
                ["Back to Tasks"] = "Volver a tareas",
                ["Edit Task"] = "Editar tarea",
                ["Delete"] = "Eliminar",
                ["Update task details, deadline, and progress."] = "Actualiza los detalles de la tarea, fecha limite y progreso.",
                ["Task Information"] = "Informacion de la tarea",
                ["Task Title"] = "Titulo de la tarea",
                ["Enter task title"] = "Escribe el titulo de la tarea",
                ["Enter task description"] = "Escribe la descripcion de la tarea",
                ["Status & Deadline"] = "Estado y fecha limite",
                ["Type a category"] = "Escribe una categoria",
                ["Task Color"] = "Color de la tarea",
                ["Task Reflection"] = "Reflexion de la tarea",
                ["Write quick notes about progress, blockers, or what you learned."] = "Escribe notas rapidas sobre progreso, bloqueos o lo aprendido.",
                ["Add your reflection here..."] = "Agrega tu reflexion aqui...",
                ["Cancel"] = "Cancelar",
                ["Save Changes"] = "Guardar cambios",
                ["Organise your tasks into clear areas of focus."] = "Organiza tus tareas en areas de enfoque claras.",
                ["+ Add Category"] = "+ Agregar categoria",
                ["Manage Categories"] = "Gestionar categorias",
                ["Total"] = "Total",
                ["Most Active"] = "Mas activa",
                ["Areas of Focus"] = "Areas de enfoque",
                ["Edit"] = "Editar",
                ["No reflection yet."] = "Aun no hay reflexion.",
                ["Manage your app preferences and account experience."] = "Administra tus preferencias y experiencia de cuenta.",
                ["Preferences"] = "Preferencias",
                ["Enable Notifications"] = "Activar notificaciones",
                ["Receive reminders about task deadlines."] = "Recibe recordatorios de fechas limite.",
                ["Dark Mode"] = "Modo oscuro",
                ["Toggle a darker appearance for the app."] = "Activa una apariencia mas oscura para la app.",
                ["Appearance & Language"] = "Apariencia e idioma",
                ["Theme"] = "Tema",
                ["Select theme"] = "Seleccionar tema",
                ["Language"] = "Idioma",
                ["Select language"] = "Seleccionar idioma",
                ["Productivity"] = "Productividad",
                ["Reminder Frequency"] = "Frecuencia de recordatorio",
                ["Choose reminder frequency"] = "Elegir frecuencia de recordatorio",
                ["Save Settings"] = "Guardar configuracion",
                ["Reset"] = "Restablecer",
                ["Account"] = "Cuenta",
                ["Sign out of your account on this device."] = "Cerrar sesion en este dispositivo.",
                ["Logout"] = "Cerrar sesion",
                ["Welcome back"] = "Bienvenido",
                ["Login to TaskNest"] = "Iniciar sesion en TaskNest",
                ["Access your tasks, categories, and productivity dashboard."] = "Accede a tus tareas, categorias y panel de productividad.",
                ["Account Login"] = "Inicio de sesion",
                ["Email Address"] = "Correo electronico",
                ["Enter your email"] = "Introduce tu correo",
                ["Password"] = "Contrasena",
                ["Enter your password"] = "Introduce tu contrasena",
                ["Remember me"] = "Recordarme",
                ["Forgot password?"] = "Olvidaste tu contrasena?",
                ["Login"] = "Iniciar sesion",
                ["Create an account"] = "Crear una cuenta",
                ["Demo login page for TaskNest mobile coursework project."] = "Pagina de inicio de sesion de demostracion para TaskNest.",
                ["Create account"] = "Crear cuenta",
                ["Join TaskNest and manage your productivity."] = "Unete a TaskNest y gestiona tu productividad.",
                ["Register"] = "Registro",
                ["Full Name"] = "Nombre completo",
                ["Enter your name"] = "Introduce tu nombre",
                ["Email"] = "Correo",
                ["Create password"] = "Crear contrasena",
                ["Confirm Password"] = "Confirmar contrasena",
                ["Confirm password"] = "Confirmar contrasena",
                ["Create Account"] = "Crear cuenta",
                ["Already have an account? Login"] = "Ya tienes cuenta? Inicia sesion",
                ["English"] = "Ingles",
                ["French"] = "Frances",
                ["Spanish"] = "Espanol",
                ["Light"] = "Claro",
                ["Dark"] = "Oscuro",
                ["System Default"] = "Sistema",
                ["Daily"] = "Diario",
                ["Weekly"] = "Semanal",
                ["Only for deadlines"] = "Solo para fechas limite",
                ["Settings saved successfully."] = "Configuracion guardada correctamente.",
                ["You have been signed out."] = "Has cerrado sesion.",
                ["OK"] = "OK"
            }
        };

    public event EventHandler? LanguageChanged;

    public string CurrentLanguageCode { get; private set; }

    private LocalizationService()
    {
        CurrentLanguageCode = Preferences.Default.Get(LanguagePreferenceKey, "en");
        if (CurrentLanguageCode != "fr" && CurrentLanguageCode != "es")
        {
            CurrentLanguageCode = "en";
        }
    }

    public void SetLanguage(string languageDisplayName)
    {
        var languageCode = languageDisplayName.Trim().ToLowerInvariant() switch
        {
            "french" or "francais" => "fr",
            "spanish" or "espanol" => "es",
            _ => "en"
        };

        if (string.Equals(CurrentLanguageCode, languageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentLanguageCode = languageCode;
        Preferences.Default.Set(LanguagePreferenceKey, CurrentLanguageCode);

        try
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(CurrentLanguageCode);
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(CurrentLanguageCode);
        }
        catch
        {
            // Ignore culture fallback failures and keep translation dictionary mode.
        }

        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }

    public string Translate(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || CurrentLanguageCode == "en")
        {
            return text;
        }

        if (_translations.TryGetValue(CurrentLanguageCode, out var map)
            && map.TryGetValue(text, out var translated))
        {
            return translated;
        }

        return text;
    }

    public void ApplyToPage(Page? page)
    {
        if (page is null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(page.Title))
        {
            page.Title = Translate(page.Title);
        }

        if (page is ContentPage contentPage)
        {
            ApplyToElement(contentPage.Content);
        }
    }

    private void ApplyToElement(Element? element)
    {
        if (element is null)
        {
            return;
        }

        if (element is Label label)
        {
            label.Text = Translate(label.Text);
        }
        else if (element is Button button)
        {
            button.Text = Translate(button.Text);
        }
        else if (element is Entry entry)
        {
            entry.Placeholder = Translate(entry.Placeholder);
        }
        else if (element is Editor editor)
        {
            editor.Placeholder = Translate(editor.Placeholder);
        }
        else if (element is Picker picker)
        {
            picker.Title = Translate(picker.Title);
        }

        switch (element)
        {
            case ContentPage cp when cp.Content is not null:
                ApplyToElement(cp.Content);
                break;
            case ScrollView sv when sv.Content is not null:
                ApplyToElement(sv.Content);
                break;
            case ContentView cv when cv.Content is not null:
                ApplyToElement(cv.Content);
                break;
            case Border border when border.Content is not null:
                ApplyToElement(border.Content);
                break;
            case Frame frame when frame.Content is not null:
                ApplyToElement(frame.Content);
                break;
            case Layout layout:
                foreach (var child in layout.Children)
                {
                    ApplyToElement(child as Element);
                }
                break;
        }
    }
}