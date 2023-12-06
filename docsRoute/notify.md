`POST` /notify

Route pour notify le système de la présence de l'utilisateur.

**Body :** 

- espId - required, string

```json
{
	"espId": "1fbsjkvn2"
}
```

**Response Code :** 

200
```json
{
	"message": "System notified",
}
```

400 :

```json
{
	"message": "Invalid request"
}
```

```json
{
	"message": "Incorrect value sent"
}
```
500:

```json
{
	"error": "Internal server error",
}
```

</aside>