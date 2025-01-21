namespace TP;

public class Program
{
    private static readonly Dictionary<string, long> RomanValues = new()
    {
        { "I", 1 },
        { "V", 5 },
        { "X", 10 },
        { "L", 50 },
        { "C", 100 },
        { "D", 500 },
        { "M", 1000 },
        { "I̅", 1000 },
        { "V̅", 5000 },
        { "X̅", 10000 },
        { "L̅", 50000 },
        { "C̅", 100000 },
        { "D̅", 500000 },
        { "M̅", 1000000 }
    };

    private static bool EstSuccessionValide(string chiffreRomain, int position)
    {
        // Obtenir le symbole actuel et suivant
        var symboleActuel = position + 1 < chiffreRomain.Length && chiffreRomain[position + 1] == '\u0305'
            ? chiffreRomain[position] + "\u0305"
            : chiffreRomain[position].ToString();

        var positionSuivante = position + (symboleActuel.Length == 2 ? 2 : 1);
        if (positionSuivante >= chiffreRomain.Length) return true;

        var symboleSuivant = positionSuivante + 1 < chiffreRomain.Length && chiffreRomain[positionSuivante + 1] == '\u0305'
            ? chiffreRomain[positionSuivante] + "\u0305"
            : chiffreRomain[positionSuivante].ToString();

        // Vérifier si c'est une soustraction
        if (!RomanValues.TryGetValue(symboleActuel, out var valeurActuelle) ||
            !RomanValues.TryGetValue(symboleSuivant, out var valeurSuivante) ||
            valeurActuelle >= valeurSuivante)
        {
            return true;
        }

        // Vérifier si la soustraction est permise entre ces symboles
        if (!((symboleActuel == "I" && symboleSuivant is "V" or "X") ||
              (symboleActuel == "X" && symboleSuivant is "L" or "C") ||
              (symboleActuel == "C" && symboleSuivant is "D" or "M")))
        {
            return false;
        }

        // Vérifier qu'il n'y a pas de répétition avant une soustraction
        if (position <= 0) return true;
        var symbolePrecedent = chiffreRomain[position - 1] == '\u0305'
            ? chiffreRomain[position - 2] + "\u0305"
            : chiffreRomain[position - 1].ToString();

        return symbolePrecedent != symboleActuel;
    }

    private static void VerifierRepetitions(string chiffreRomain)
    {
        // Vérifie les répétitions de symboles
        for (var i = 0; i < chiffreRomain.Length - 3; i++)
        {
            if (chiffreRomain[i] == chiffreRomain[i + 1] && 
                chiffreRomain[i] == chiffreRomain[i + 2] && 
                chiffreRomain[i] == chiffreRomain[i + 3])
            {
                throw new ArgumentException($"Le symbole '{chiffreRomain[i]}' ne peut pas être répété plus de trois fois");
            }
        }
    }

    public static long ConvertirChiffreRomain(string chiffreRomain)
    {
        if (string.IsNullOrEmpty(chiffreRomain))
            throw new ArgumentException("Le chiffre romain ne peut pas être vide");

        chiffreRomain = chiffreRomain.ToUpper();
        VerifierRepetitions(chiffreRomain);
        
        var resultat = 0L;
        var i = 0;

        while (i < chiffreRomain.Length)
        {
            if (!EstSuccessionValide(chiffreRomain, i))
            {
                throw new ArgumentException("Succession de symboles invalide dans le chiffre romain");
            }

            var symboleActuel = i + 1 < chiffreRomain.Length && chiffreRomain[i + 1] == '\u0305' 
                ? chiffreRomain[i] + "\u0305"
                : chiffreRomain[i].ToString();

            if (!RomanValues.TryGetValue(symboleActuel, out var valeurActuelle))
            {
                throw new ArgumentException($"Caractère invalide dans le chiffre romain: {symboleActuel}");
            }

            i += symboleActuel.Length;

            if (i < chiffreRomain.Length)
            {
                var symboleSuivant = i + 1 < chiffreRomain.Length && chiffreRomain[i + 1] == '\u0305'
                    ? chiffreRomain[i] + "\u0305"
                    : chiffreRomain[i].ToString();

                if (RomanValues.TryGetValue(symboleSuivant, out var valeurSuivante) && valeurSuivante > valeurActuelle)
                {
                    resultat += valeurSuivante - valeurActuelle;
                    i += symboleSuivant.Length;
                    continue;
                }
            }

            resultat += valeurActuelle;
        }

        return resultat;
    }

    private static void Main()
    {
        while (true)
        {
            Console.WriteLine("Entrez un nombre romain (ou 'quitter' pour arrêter):");
            var input = Console.ReadLine();

            if (input?.ToLower() == "quitter")
            {
                break;
            }

            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    var resultat = ConvertirChiffreRomain(input);
                    Console.WriteLine($"Le nombre {input} en chiffres arabes est: {resultat:N0}");
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine($"Erreur: {e.Message}");
            }
            Console.WriteLine();
        }
        
        Console.WriteLine("Au revoir !");
    }
}