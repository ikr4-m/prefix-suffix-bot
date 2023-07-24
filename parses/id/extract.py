#/([()a-z0-9_ .,'\/-]*\w)/gim
import re
import json
from re import Match

file = open('raw.txt', mode='rb')
files = file.readlines()
file.close()

files = [i.decode('utf-8', 'ignore') for i in files]
files = [re.match(r"[\x20-\x7E]+", i) for i in files]
files = [i.group() for i in files if type(i) == Match]

json = json.dumps(files)

wr_file = open('raw.json', 'w')
wr_file.write(json)
wr_file.close()