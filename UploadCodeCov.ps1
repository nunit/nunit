(New-Object System.Net.WebClient).DownloadFile("https://codecov.io/bash", ".\CodecovUploader.sh")
.\CodecovUploader.sh -f "opencover4.5.xml" -t "72811e91-101d-439b-aed1-52f6297b0976" -X gcov
.\CodecovUploader.sh -f "opencover3.5.xml" -t "72811e91-101d-439b-aed1-52f6297b0976" -X gcov
.\CodecovUploader.sh -f "opencover3.0.xml" -t "72811e91-101d-439b-aed1-52f6297b0976" -X gcov
.\CodecovUploader.sh -f "opencover2.0.xml" -t "72811e91-101d-439b-aed1-52f6297b0976" -X gcov