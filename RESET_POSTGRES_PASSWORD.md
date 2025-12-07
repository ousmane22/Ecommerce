# Réinitialiser le mot de passe PostgreSQL

## Méthode 1 : Via pgAdmin ou un outil graphique
1. Ouvrez pgAdmin
2. Connectez-vous avec votre mot de passe actuel
3. Clic droit sur le serveur → Properties → Change Password

## Méthode 2 : Via la ligne de commande (Windows)
1. Arrêtez le service PostgreSQL
2. Modifiez le fichier `pg_hba.conf` (généralement dans `C:\Program Files\PostgreSQL\[version]\data\`)
3. Changez `md5` en `trust` pour la ligne `host all all 127.0.0.1/32`
4. Redémarrez PostgreSQL
5. Connectez-vous sans mot de passe : `psql -U postgres`
6. Changez le mot de passe : `ALTER USER postgres PASSWORD 'votre_nouveau_mot_de_passe';`
7. Remettez `md5` dans `pg_hba.conf`
8. Redémarrez PostgreSQL

## Méthode 3 : Utiliser un autre utilisateur
Si vous avez un autre utilisateur PostgreSQL avec un mot de passe connu, vous pouvez l'utiliser dans la chaîne de connexion.

