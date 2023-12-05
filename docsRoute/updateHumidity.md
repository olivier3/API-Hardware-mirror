`POST` /updateHumidity

Route pour update la l'humidité.

**Body :** 

- espId - required, string
- humidity - required, double

```json
{
    "espId" : "1cbsijk34vnsk2",
    "humidity" : 21.34
}
```

**Response Code :** 

200
```json
{
	"message": "Humidity updated"
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
	"message": "Incorrect value type"
}
```

500:

```json
{
	"error": "Internal server error",
}
```

</aside>