import direct
import sys
dataFileName = sys.argv[1]
textFileName = sys.argv[2]
dataModule = dataFileName.split('.py')[0]
print 'Parsing %s.py --> %s\n' % (dataModule, textFileName)
globals().update(__import__('pirates.leveleditor.worldData.' + dataModule, fromlist=['*']).__dict__)
lines = []
for mainUid in objectStruct['Objects']:
    mainObj = objectStruct['Objects'][mainUid]
    lines.append('Name:\t%s\t%s\nType:\t%s\n\n' % (mainObj['Name'], mainUid, mainObj['Type']))
    for uid in mainObj['Objects']:
        object = mainObj['Objects'][uid]
        lines.append('%s\t%s\t%s\n' % (uid, object['Type'], `object['Pos']`))

    lines.append('\n')

textFile = file(textFileName, 'w')
textFile.writelines(lines)
textFile.close()
