#!/usr/bin/python

import json, sys, getopt, os, re

def usage():
  print("Usage: %s --file=[filename]" % sys.argv[0])
  sys.exit()

def main(argv):

  file=''
 
  myopts, args = getopt.getopt(sys.argv[1:], "", ["file="])
 
  for o, a in myopts:
    if o in ('-f, --file'):
      file=a
    else:
      usage()

  if len(file) == 0:
    usage()
 
  corpus = open(file)
  urldata = json.load(corpus, encoding="latin1")
  numUrls = 0
  maliciousCount = 0
  actualMalicious = 0
  thresholdTotal = 400
  error = 0
  
  for record in urldata:
    threshold = 0
    numUrls = numUrls + 1
    malicious = 0
    regexGoogle = re.search("[^www\.)]google(docs|doc|drive|mail|plus|calendar)*", record["url"]) #Does this look like a google address?
    regexIp = re.match("^(\.[0-9][0-9]?[0-9])+$", record["host"])
    
    if regexIp:
      threshold = threshold + 500

    if record["scheme"] == "https":
      threshold = threshold - 300
    
    domainAge = int(record['domain_age_days'])
    if domainAge < 360:
      threshold += 600

    ext = record["file_extension"]

    if(ext in ["zip", "php"]):
      threshold = threshold + 400
    elif ext == "exe":
      threshold = threshold + 300
    elif ext in ["aspx", "asp" "xml", "de"]:
      threshold = threshold + 300
    
    if record["alexa_rank"] == None:
      threshold = threshold + 0
    elif record["alexa_rank"] < 100000:
      threshold = threshold - 300
          
    if regexGoogle:
      threshold += 700

    if record["malicious_url"]:
      actualMalicious = actualMalicious + 1

    if threshold > thresholdTotal:
      maliciousCount = maliciousCount + 1
      malicious = 1
    if malicious != record["malicious_url"]:
      print record["url"], malicious, record["malicious_url"]
      error = error + 1

  print "actual malicious:", actualMalicious
  print "identified malicious:", maliciousCount
  print "errors:", error

  corpus.close()

if __name__ == "__main__":
  main(sys.argv[1:])
