from pandac.PandaModules import Point3, VBase3, Vec4
objectStruct = {
    'Objects': {
        '1209584000.0WDIG': {
            'Type': 'Ship Part',
            'Name': 'shipFrenchInterceptorL3',
            'Category': '125: French Shadow Crow',
            'File': '',
            'Flagship': False,
            'Objects': {
                '1209584256.0WDIG1': {
                    'Type': 'Spawn Node',
                    'AnimSet': 'default',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Min Population': '1',
                    'Patrol Radius': '12.0000',
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(-0.838, 0.97798, 21.809),
                    'PoseAnim': '',
                    'PoseFrame': '',
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'French Undead',
                    'Start State': 'Patrol',
                    'StartFrame': '0',
                    'Team': '1',
                    'TrailFX': 'None',
                    'Visual': {
                        'Color': (0, 0, 0.65, 1),
                        'Model': 'models/misc/smiley' } },
                '1209584256.0WDIG2': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(-0.542000, -12.118, 21.786),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1209584384.0WDIG': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(-0.35298, 44.783, 22.637),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1211501568.0WDIG1': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': 100,
                    'Pause Duration': 30,
                    'Pos': Point3(-1.091, -63.700, 23.85),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1211501568.0WDIG2': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(17.649, -33.581, 22.123),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1211501568.0WDIG3': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(-15.194, -33.536, 22.123),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1211501568.0WDIG4': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(-24.052, 1.838, 21.812),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } },
                '1211501568.0WDIG5': {
                    'Type': 'Movement Node',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pause Chance': '100',
                    'Pause Duration': '30',
                    'Pos': Point3(19.631, 3.1908, 21.643),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.65, 0, 0, 1),
                        'Model': 'models/misc/smiley' } } },
            'Respawns': True,
            'Team': 'Undead',
            'Visual': {
                'Model': [
                    'models/shipparts/-geometry_High',
                    'models/shipparts/skeletonInterceptorL3-collisions',
                    'models/shipparts/skeletonInterceptorL3-collisions',
                    'models/shipparts/skeletonInterceptorL3-geometry_High'] } } },
    'Node Links': [
        [
            '1209584256.0WDIG2',
            '1209584256.0WDIG1',
            'Bi-directional'],
        [
            '1209584384.0WDIG',
            '1209584256.0WDIG1',
            'Bi-directional'],
        [
            '1209584256.0WDIG2',
            '1211501568.0WDIG1',
            'Bi-directional'],
        [
            '1211501568.0WDIG5',
            '1209584384.0WDIG',
            'Bi-directional'],
        [
            '1211501568.0WDIG4',
            '1209584384.0WDIG',
            'Bi-directional'],
        [
            '1211501568.0WDIG2',
            '1211501568.0WDIG5',
            'Bi-directional'],
        [
            '1211501568.0WDIG4',
            '1211501568.0WDIG3',
            'Bi-directional'],
        [
            '1211501568.0WDIG1',
            '1211501568.0WDIG3',
            'Bi-directional'],
        [
            '1211501568.0WDIG2',
            '1211501568.0WDIG1',
            'Bi-directional'],
        [
            '1211501568.0WDIG4',
            '1211501568.0WDIG5',
            'Bi-directional'],
        [
            '1211501568.0WDIG2',
            '1211501568.0WDIG3',
            'Bi-directional'],
        [
            '1211501568.0WDIG4',
            '1209584256.0WDIG2',
            'Bi-directional'],
        [
            '1209584256.0WDIG2',
            '1211501568.0WDIG3',
            'Bi-directional'],
        [
            '1211501568.0WDIG2',
            '1209584256.0WDIG2',
            'Bi-directional'],
        [
            '1209584256.0WDIG2',
            '1211501568.0WDIG5',
            'Bi-directional']],
    'Layers': { },
    'ObjectIds': {
        '1209584000.0WDIG': '["Objects"]["1209584000.0WDIG"]',
        '1209584256.0WDIG1': '["Objects"]["1209584000.0WDIG"]["Objects"]["1209584256.0WDIG1"]',
        '1209584256.0WDIG2': '["Objects"]["1209584000.0WDIG"]["Objects"]["1209584256.0WDIG2"]',
        '1209584384.0WDIG': '["Objects"]["1209584000.0WDIG"]["Objects"]["1209584384.0WDIG"]',
        '1211501568.0WDIG1': '["Objects"]["1209584000.0WDIG"]["Objects"]["1211501568.0WDIG1"]',
        '1211501568.0WDIG2': '["Objects"]["1209584000.0WDIG"]["Objects"]["1211501568.0WDIG2"]',
        '1211501568.0WDIG3': '["Objects"]["1209584000.0WDIG"]["Objects"]["1211501568.0WDIG3"]',
        '1211501568.0WDIG4': '["Objects"]["1209584000.0WDIG"]["Objects"]["1211501568.0WDIG4"]',
        '1211501568.0WDIG5': '["Objects"]["1209584000.0WDIG"]["Objects"]["1211501568.0WDIG5"]' } }
extraInfo = {
    'camPos': Point3(-124.52, 9.32657, 238.828),
    'camHpr': VBase3(-94.9689, -57.7601, -3.20085e-006),
    'focalLength': 1.3999999761599999,
    'skyState': 2,
    'fog': 0 }
