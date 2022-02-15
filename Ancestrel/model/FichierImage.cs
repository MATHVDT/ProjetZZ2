﻿/**
 * @file Image.cs
 * Fichier contenant la classe Image
 * @author Mathieu
 * @date 31/12/2021
 * @copyright ...
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Model
{
    /**
     * @class FichierImage
     * @brief Classe pour les fichiers de type images.
     */
    public sealed class FichierImage : Fichier
    {
        private readonly Image _image;
        public Image Image { get { return _image; } }


        /**
         * @fn public FichierImage(string filename, string nomFichier)
         * @brief Constructeur d'un FichierImage
         * @param string pathFile  - *Chemin de l'image à charger*
         * @param int? id = null - *Id du fichier dans la bdd*
         * @param  string nomFichier = "" - *Nom du fichier*
         */
        public FichierImage(string pathFile, int? id = null, string nomFichier = "") : base(id, nomFichier)
        {
            try
            {
                _image = Image.FromFile(pathFile);
            }
            catch (TypeInitializationException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("using System.Drawing : bibliothèque spécifique à Windows ");
            }
            catch (OutOfMemoryException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("Format d'image du fichier n'est pas valide.");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Le fichier spécifié n'existe pas.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("ArgumentException");
            }
            if (_image == null)
            {
                Console.WriteLine("image non chargée");
                throw new Exception("Image non chargée");
            }
        }

        /**
         * overload public FichierImage(string pathFile, string nomFichier = "")
         * @brief Constructeur d'une nouvelle image. *sans id*
         */
        public FichierImage(string pathFile, string nomFichier = "")
            : this(pathFile, null, nomFichier) { }

        /**
         * @overload public FichierImage()
         * 
         * @param byte[] imgByte - *Image en tableau de byte*
         */
        public FichierImage(byte[] imgByte, int? id = null, string? nomFichier = null, DateTime? dateAjout = null)
            : base(dateAjout, id, nomFichier)
        {
            _image = ByteArrayToImage(imgByte);
        }


        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }


    }
}
