`POST` /updateTemp

Route pour update la temperature.

**Body :** 

- espId - required, string
- temperature - required, double

```json
{
    "espId" : "1cbsijk34vnsk2",
    "temperature" : 21.34
}
```

**Response Code :** 

200
```json
{
	"message": "Temperature updated"
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