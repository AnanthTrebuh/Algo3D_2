# Algo3D Jalon 3 Hubert & Lenglet 

## Interface

Canvas avec la scene à afficher et l'objet qui est le Bunny par défault

Une interface javascript de contrôle permettant de manipuler les différent paramètre se trouve sur la droite supérieur de l'écran.

Vous pouvez activer la checkbox "Miroir" pour activer le miroir.  
Une seconde checkbox apparait pour le miroir dépoli. Les slider rugosité et nombre d'échnatillon permet de modifier le rendu.

Vous pouvez activer la checkbox "Refraction", qui activera le miroir en même temps (demander par M. Meneveaux) et désactivera le miroir dépolie, pour observer les deux effet en même temps.

Une checkbox pour l'effet de cook et torrence est activable avec une lumière posé en 0,0,0.  
Quand vous activer Cook et torrence le miroir et la refraction se désactive s'il sont activés.  

Pour déplacer la lumière il suffit de maintenir la touche "ctrl" et de déplacer la souris en cliquant sur la scène.

Une checknbox pour l'échantillonage d'importance est activable, les slider de l'indice de réfraction, de la rugosité et du nombre d'échantillon permet de modifier le rendu

Vous avez en dessous de toutes les checkbox quatres sliders.  
Le premier sert à modifier le taux de réfraction ni qui se trouve entre 1,0 et 3,0.   
Le second sert à modifier le taux de rugosité sigma qui se trouve entre 0,1 et 0.5.

Le troisième sert à modifier l'intensité du rendu, recommandé pour l'utilisation de l'échantillonage d'importance, moins pour les autres. 

Le quatrième sert à modifier le nombre d'échantillon tirés pour le miroir dépoli et l'échantillonage d'importance.

Vous avez ensuite un Color Chooser pour modifier la couleur de l'objet Kd. 

Vous pouvez modifier l'univers de la cubemap en choisissant l'image qui vous plait le plus. 

Vous avez une list dropdown pour sélectionner l'objet à afficher. 
