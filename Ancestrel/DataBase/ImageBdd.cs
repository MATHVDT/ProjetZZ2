﻿using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    internal class ImageBdd
    {


        private readonly string _chaineConnexion;

        private readonly string _ImageTable = "dbo.Image";
        private readonly string _ImagePersonneTable = "dbo.Image_Personne";

        private readonly string _Id = "Id";
        private readonly string _Image = "Image";
        private readonly string _Nom = "Nom";
        private readonly string _DateAjout = "DateAjout";

        private readonly string _IdImage = "Id_image";
        private readonly string _IdPersonne = "Id_personne";


        public ImageBdd(string chaineConnexion)
        {
            _chaineConnexion = chaineConnexion;
        }


        /**
         * @fn public FichierImage GetVilleTableById
         * @brief Récupère dans la bdd une image par son Id.
         * 
         * @param int idImage - *Id de l'image à récupérer*
         * 
         * @return FichierImage *Image chargée depuis la bdd*
         */
        public FichierImage GetImageById(int idImage)
        {
            FichierImage fichierImage = null;


            // Requete SQL pour récuperer les infos sur une ville
            string queryString = $"SELECT * " +
                                 $"FROM {_ImageTable} " +
                                 $"WHERE {_Id} = {idImage};";

            // Connexion à la bdd
            SqlConnection connexion = new SqlConnection(_chaineConnexion);

            try
            {
                connexion.Open(); // Ouverture de la connexion à la bdd

                // Création et execution de la requete SQL
                Console.WriteLine(queryString);
                SqlCommand commandSql = new SqlCommand(queryString, connexion);
                SqlDataReader reader = commandSql.ExecuteReader();

                reader.Read(); // Ouverture du reader

                int idBdd = (int)reader[_Id];
                string nomBdd = (string)reader[_Nom];
                DateTime? dateAjout = (DateTime?)(reader[_DateAjout] is System.DBNull ? null : (DateTime)reader[_DateAjout]);



                byte[] imgByteBdd = (byte[])reader[_Image];
    
                //foreach (var x in imgByteBdd)
                //    Console.Write(x);


                reader.Close(); // Fermeture du reader

                // Création du fichier image
                fichierImage = new FichierImage(imgByteBdd,
                                                idBdd, nomBdd,
                                                dateAjout);
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error SQL Generated. Details: " + e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Generated. Details: " + e.ToString());
            }
            finally
            {
                connexion.Close();
            }

            return fichierImage;
        }


        public void InsererImageTable(FichierImage fichierImage)
        {
            string values = ImageValuesInsert(fichierImage);


            // Connexion à la bdd
            SqlConnection connexion = new SqlConnection(_chaineConnexion);
            SqlCommand commandSql;
            string queryString;

            try
            {
                connexion.Open(); // Ouverture connexion

                // Requete SQL pour inserer l'image
                queryString = $"INSERT INTO {_ImageTable} \n" +
                              $"({_Nom}, {_DateAjout}, {_Image} ) \n" +
                              $"VALUES ({values});";

                // Création de la requete SQL d'INSERTION
                commandSql = new SqlCommand(queryString, connexion);

                // Excecution de l'insertion
                Console.WriteLine(queryString);
                Console.WriteLine(commandSql.ExecuteNonQuery() + " ligne inserée.");

                // Requete SQL pour récupérer l'id de l'image
                queryString = $"SELECT IDENT_CURRENT('{_ImageTable}');";

                // Création de la requete SQL pour récupérer l'Id attribué àl'image inserée
                commandSql = new SqlCommand(queryString, connexion);

                // Excecution de la requete de récupération de l'Id
                Console.WriteLine(queryString);
                SqlDataReader reader = commandSql.ExecuteReader();
                reader.Read();
                int idAutoIncrementeImage = Convert.ToInt32(reader[0]);
                fichierImage.Id = idAutoIncrementeImage;
                Console.WriteLine("Id auto incrementé de la personne : " + idAutoIncrementeImage);

                reader.Close(); // Fermeture du reader

            }
            catch (SqlException e)
            {
                Console.WriteLine("Error SQL Generated. Details: " + e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Generated. Details: " + e.ToString());
            }
            finally
            {
                connexion.Close();
            }
        }

        /**
         * public string ImageValuesInsert
         * @brief Récupère et donne les valeurs des champs à inserer dans la bdd.
         * 
         * @param FichierImage fichierImage
         * 
         * @return string *Valeurs à inserer*
         */
        public string ImageValuesInsert(FichierImage fichierImage)
        {
            // Récupération des champs du fichierImage
            //const string VALUENULL = "NULL";

            StringBuilder valuesBuilder = new StringBuilder();

            valuesBuilder.Append($"'{fichierImage.NomFichier}', ");

            valuesBuilder.Append($"{$"CONVERT(date, '{fichierImage.DateAjoutFichier}', 103)"}, ");

            byte[] tmp = FichierImage.ImageToByteArray(fichierImage.Image);

            valuesBuilder.Append("CONVERT(VARBINARY(MAX), '");

            foreach (var x in tmp)
                valuesBuilder.Append($"{x.ToString()}");

            valuesBuilder.Append("' ) ");

            return valuesBuilder.ToString();
        }
    }
}
