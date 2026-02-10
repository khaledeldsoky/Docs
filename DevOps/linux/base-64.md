✅ Encode a String to Base64
```sh
echo -n "my-secret-key" | base64
```

✅ Encode a File to Base64
```sh
base64 config.json
```

✅ If you want to save the output to another file, use:
```sh
base64 config.json > config.b64
```

✅ Decode Base64 Back to Normal Text
```sh
echo "bXktc2VjcmV0LWtleQ==" | base64 --decode
```

✅ For a Base64-encoded file
```sh
base64 --decode config.b64 > config.json
```
26d607f0-fa81-413e-8e37-4ac21171d315