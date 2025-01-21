using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TP2;

internal abstract class Program
{
    private static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nChoisissez une option :");
            Console.WriteLine("1. Encoder du texte dans une image");
            Console.WriteLine("2. Décoder du texte depuis une image");
            Console.WriteLine("3. Quitter");

            var choix = Console.ReadLine();
            switch (choix)
            {
                case "1":
                    EncoderTexte();
                    break;
                case "2":
                    DecoderTexte();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Option invalide");
                    break;
            }
        }
    }

    private static void EncoderTexte()
    {
        Console.WriteLine("Entrez le chemin de l'image noir et blanc à modifier :");
        var cheminImage = Console.ReadLine();

        if (!File.Exists(cheminImage))
        {
            Console.WriteLine("L'image n'existe pas.");
            return;
        }

        Console.WriteLine("Entrez le texte à encoder :");
        var texte = Console.ReadLine();

        if (string.IsNullOrEmpty(texte))
        {
            Console.WriteLine("Le texte ne peut pas être vide.");
            return;
        }

        try
        {
            // Convertir le texte en bytes UTF-8
            var bytes = System.Text.Encoding.UTF8.GetBytes(texte);

            using var imageOriginale = Image.Load<L8>(cheminImage);
            var capaciteImage = imageOriginale.Width * imageOriginale.Height;

            if (bytes.Length > capaciteImage - 4)
            {
                Console.WriteLine($"Le texte est trop long pour cette image. Maximum: {capaciteImage - 4} bytes");
                return;
            }

            using var imageEncodee = imageOriginale.Clone();

            // Encoder la longueur des bytes dans les 4 premiers pixels en modifiant seulement le bit de poids faible
            for (var i = 0; i < 4; i++)
            {
                var longueurByte = (byte)(bytes.Length >> (i * 8));
                var originalValue = imageOriginale[i, 0].PackedValue;
                // Garder les 7 bits de poids fort et modifier seulement le dernier bit
                var newValue = (byte)((originalValue & 0xFE) | (longueurByte & 0x01));
                imageEncodee[i, 0] = new L8(newValue);
            }

            // Encoder les bytes du texte à partir du 5ème pixel
            for (var i = 0; i < bytes.Length; i++)
            {
                var x = (i + 4) % imageEncodee.Width;
                var y = (i + 4) / imageEncodee.Width;

                var originalValue = imageOriginale[x, y].PackedValue;
                // Garder les 7 bits de poids fort et modifier seulement le dernier bit
                var newValue = (byte)((originalValue & 0xFE) | (bytes[i] & 0x01));
                imageEncodee[x, y] = new L8(newValue);
            }

            var nomFichierSortie = Path.GetFileNameWithoutExtension(cheminImage) + "_encoded" +
                                   Path.GetExtension(cheminImage);
            imageEncodee.Save(nomFichierSortie);

            Console.WriteLine($"Image modifiée sauvegardée sous : {nomFichierSortie}");
            Console.WriteLine($"\nInformations sur l'encodage :");
            Console.WriteLine($"Dimensions de l'image : {imageEncodee.Width}x{imageEncodee.Height} pixels");
            Console.WriteLine($"Nombre de bytes encodés : {bytes.Length}");
            Console.WriteLine($"Espace disponible restant : {capaciteImage - bytes.Length - 4} pixels");

            // Afficher les bits modifiés pour le débogage
            Console.WriteLine("\nBits modifiés (premiers pixels) :");
            for (var i = 0; i < Math.Min(10, bytes.Length + 4); i++)
            {
                var x = i % imageEncodee.Width;
                var y = i / imageEncodee.Width;
                var originalBit = imageOriginale[x, y].PackedValue & 0x01;
                var newBit = imageEncodee[x, y].PackedValue & 0x01;
                Console.WriteLine($"Pixel [{x},{y}] : Bit original={originalBit}, Nouveau bit={newBit}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du traitement de l'image : {ex.Message}");
        }
    }

    private static void DecoderTexte()
    {
        Console.WriteLine("Entrez le chemin de l'image originale :");
        var cheminImageOriginale = Console.ReadLine();

        if (!File.Exists(cheminImageOriginale))
        {
            Console.WriteLine("L'image originale n'existe pas.");
            return;
        }

        Console.WriteLine("Entrez le chemin de l'image encodée :");
        var cheminImageEncodee = Console.ReadLine();

        if (!File.Exists(cheminImageEncodee))
        {
            Console.WriteLine("L'image encodée n'existe pas.");
            return;
        }

        try
        {
            using var imageOriginale = Image.Load<L8>(cheminImageOriginale);
            using var imageEncodee = Image.Load<L8>(cheminImageEncodee);

            if (imageOriginale.Width != imageEncodee.Width || imageOriginale.Height != imageEncodee.Height)
            {
                Console.WriteLine("Les dimensions des images ne correspondent pas.");
                return;
            }

            // Lire la longueur du texte depuis les 4 premiers pixels
            var longueurTexte = 0;
            for (var i = 0; i < 4; i++)
            {
                // Extraire le bit de poids faible
                var bit = imageEncodee[i, 0].PackedValue & 0x01;
                longueurTexte |= bit << (i * 8);
            }

            var capaciteImage = imageEncodee.Width * imageEncodee.Height;
            if (longueurTexte <= 0 || longueurTexte > capaciteImage - 4)
            {
                Console.WriteLine(
                    $"L'image ne semble pas contenir de texte encodé valide. Longueur détectée: {longueurTexte}");
                return;
            }

            // Créer un tableau pour stocker les bytes
            var bytes = new byte[longueurTexte];
            for (var i = 0; i < longueurTexte * 8; i++)
            {
                var byteIndex = i / 8;
                var bitPosition = i % 8;

                var x = ((i / 8) + 4) % imageEncodee.Width;
                var y = ((i / 8) + 4) / imageEncodee.Width;

                // Extraire le bit de poids faible
                var bit = imageEncodee[x, y].PackedValue & 0x01;
                bytes[byteIndex] |= (byte)(bit << bitPosition);
            }

            var texteDecode = System.Text.Encoding.UTF8.GetString(bytes);

            Console.WriteLine("\nTexte décodé :");
            Console.WriteLine(texteDecode);
            Console.WriteLine("\nInformations sur le décodage :");
            Console.WriteLine($"Dimensions de l'image : {imageEncodee.Width}x{imageEncodee.Height} pixels");
            Console.WriteLine($"Nombre de bytes décodés : {longueurTexte}");

            // Afficher les valeurs des bytes pour le débogage
            Console.WriteLine("\nValeurs des bytes (en hexadécimal) :");
            for (var i = 0; i < bytes.Length; i++)
            {
                Console.Write($"{bytes[i]:X2} ");
                if ((i + 1) % 16 == 0) Console.WriteLine();
            }

            // Afficher les bits extraits pour le débogage
            Console.WriteLine("\nBits extraits (premiers pixels) :");
            for (var i = 0; i < Math.Min(10, longueurTexte + 4); i++)
            {
                var x = i % imageEncodee.Width;
                var y = i / imageEncodee.Width;
                var bit = imageEncodee[x, y].PackedValue & 0x01;
                Console.WriteLine($"Pixel [{x},{y}] : Bit extrait={bit}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du décodage de l'image : {ex.Message}");
        }
    }
}