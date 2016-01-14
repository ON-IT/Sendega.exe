# Sendega - Send SMS from the command line

## Usage

``` 
Usage: Sendega.exe -u SID -p password -s sender -d destination -m message

In order to send the same SMS to multiple destinations you can either:
1) Specify the destinations separated by , or ;
2) Specify multiple destinations by appending -d DESTINATION multiple times

Options:
  -u, --username=VALUE       Sendega username
  -p, --password=VALUE       Sendega password
  -s, --sender=VALUE         Sender
  -d, --destination=VALUE    Destination
  -m, --message=VALUE        Message
      --pricegroup=VALUE     Price group
      --contenttype=VALUE    Content type ID
      --contentheader=VALUE  Content header
      --deliveryurl=VALUE    Delivery URL
      --agelimit             Age limit
      --senddate             Send date
      --priority             Priority
      --extid
      --refid
      --gwid
      --pid
      --dcs
  -h, --help                 Help

Go to http://www.sendega.com to create an account.
Created by ON IT AS <post@on-it.no> 2016.
```
