`POST` /link

Route pour link le esp32 au miroir.

**Body :** 

- userId - required, int
- espId - required, string

```json
{
    "userId": 1,
	"espId": "1fbsjkvn2"
}
```

**Response Code :** 

200
```json
{
	"temperature": 23,
	"humidity": 40
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
```json
{
	"message": "No esp data found"
}
```
500:

```json
{
	"error": "Internal server error",
}
```

</aside>