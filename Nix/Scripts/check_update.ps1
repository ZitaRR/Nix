$ErrorActionPreference = "Stop"
$UPSTREAM = if ($args.Length -le 0) {"origin/master"} else {"origin/" + $args[0]}

try{
    git fetch origin
    $local = git rev-parse HEAD
    $remote = git rev-parse $UPSTREAM
}
catch{
    "ERROR"
    exit
}

if($local -eq $remote){
    "Up to date"
    exit
}

"Update available"