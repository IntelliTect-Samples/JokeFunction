
$jokes = get-content jokes.json | ConvertFrom-Json

for ($i = 1; $i -le $jokes.count - 1; $i++) { 
    $i; Invoke-restMethod -Method post -Uri https://jokefunction396.azurewebsites.net/api/AddJoke -Body $($jokes[$i] | ConvertTo-Json) 
}
