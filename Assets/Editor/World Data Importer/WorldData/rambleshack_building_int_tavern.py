from pandac.PandaModules import Point3, VBase3, Vec4
objectStruct = {
    'Objects': {
        '1121212983.08Shochet0': {
            'Type': 'Building Interior',
            'Name': 'Tavern',
            'Instanced': True,
            'Objects': {
                '1154731709.64jubutler': {
                    'Type': 'Townsperson',
                    'Category': 'Cast',
                    'AnimSet': 'tut_dan_idle',
                    'CustomModel': 'None',
                    'DNA': '1154731709.64jubutler',
                    'Hpr': VBase3(180.0, 0.0, 0.0),
                    'Patrol Radius': 12,
                    'Pos': Point3(1.5, 34.837, 1.082),
                    'PoseAnim': '',
                    'PoseFrame': '',
                    'Respawns': True,
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Start State': 'Idle',
                    'Team': 'Player' },
                '1165268405.64kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(1.221, -1.5129, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/table_bar_round' } },
                '1165268489.64kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-43.0, -6.9340, 1.022),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/table_bar_round' } },
                '1165268495.0kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(20.657, 10.41, 0.97298),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.69999998807907104, 0.69999998807907104, 1.0),
                        'Model': 'models/props/table_bar_square' } },
                '1165268541.81kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(20.727, 7.2670, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.60000002384185791, 0.60000002384185791, 0.60000002384185791, 1.0),
                        'Model': 'models/props/stool_bar' } },
                '1165268554.8kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(-40.012, 0.0, 0.0),
                    'Pos': Point3(20.82, 13.042, 0.990),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.80000001192092896, 0.80000001192092896, 1.0),
                        'Model': 'models/props/stool_bar' } },
                '1165268615.13kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(22.064, 23.617, 0.91300),
                    'Scale': VBase3(0.617, 0.617, 0.617),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel' } },
                '1165268794.17kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(23.940, 21.495, 1.0),
                    'Scale': VBase3(0.57496, 0.57496, 0.57496),
                    'Visual': {
                        'Color': (0.30000001192092896, 0.30000001192092896, 0.30000001192092896, 1.0),
                        'Model': 'models/props/barrel' } },
                '1165269869.89kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(-1.3089, 0.0, 0.0),
                    'Pos': Point3(7.7389, 29.905, 12.523),
                    'Scale': VBase3(0.46, 0.46, 0.46),
                    'Visual': {
                        'Color': (0.74901962280273438, 0.7137255072593689, 0.60000002384185791, 1.0),
                        'Model': 'models/props/barrel_worn' } },
                '1165270073.72kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(-1.3089, 0.0, 0.0),
                    'Pos': Point3(4.298, 30.045, 12.288),
                    'Scale': VBase3(0.97498, 0.97498, 0.97498),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel_grey' } },
                '1165270537.52kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': VBase3(-1.622, 0.0, 0.0),
                    'Pos': Point3(0.78800, 31.562, 12.225),
                    'Scale': VBase3(1.222, 1.222, 1.222),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/crate' } },
                '1165270634.13kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-11.605, 30.760, 11.853),
                    'Scale': VBase3(1.705, 1.705, 1.705),
                    'Visual': {
                        'Model': 'models/props/winebottle_A' } },
                '1165270678.5kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-20.9228, 40.3178, 4.617),
                    'Scale': VBase3(1.705, 1.705, 1.705),
                    'Visual': {
                        'Model': 'models/props/winebottle_A' } },
                '1165270699.58kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-13.352, 30.733, 11.581),
                    'Scale': VBase3(1.558, 1.558, 1.558),
                    'Visual': {
                        'Model': 'models/props/winebottle_B' } },
                '1165270724.27kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': VBase3(-91.954, 1.6839, 0.0),
                    'Pos': Point3(-17.093, 31.4868, 11.741),
                    'Scale': VBase3(1.075, 1.075, 1.075),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/crate_04' } },
                '1165270820.19kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(91.5618, -5.921, 0.0),
                    'Pos': Point3(-23.123, 30.997, 11.497),
                    'Scale': VBase3(0.70896, 0.70896, 0.70896),
                    'Visual': {
                        'Color': (0.74901962280273438, 0.7137255072593689, 0.60000002384185791, 1.0),
                        'Model': 'models/props/barrel_sideways' } },
                '1165270875.49kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(82.3314, 1.2709, -0.288),
                    'Pos': Point3(-28.725, 31.291, 11.032),
                    'Scale': VBase3(0.70896, 0.70896, 0.70896),
                    'Visual': {
                        'Color': (0.47058823704719543, 0.47058823704719543, 0.47058823704719543, 1.0),
                        'Model': 'models/props/barrel_sideways' } },
                '1165270982.7kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(91.168, 0.0, 0.0),
                    'Pos': Point3(-7.1988, 31.254, 12.192),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.74901962280273438, 0.7137255072593689, 0.60000002384185791, 1.0),
                        'Model': 'models/props/barrel_sideways' } },
                '1165271086.06kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(35.020, 0.0, 0.0),
                    'Pos': Point3(27.347, 38.546, 1.0),
                    'Scale': VBase3(0.595, 0.595, 0.595),
                    'Visual': {
                        'Color': (0.89803922176361084, 0.80392158031463623, 0.69411766529083252, 1.0),
                        'Model': 'models/props/barrel_group_2' } },
                '1165271282.78kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-170.935, 0.0, 49.8328),
                    'Pos': Point3(-13.189, 40.89, 7.014),
                    'Scale': VBase3(1.2849, 1.2849, 1.2849),
                    'Visual': {
                        'Color': (0.5899999737739563, 0.52999997138977051, 0.44999998807907104, 1.0),
                        'Model': 'models/props/beerstein' } },
                '1165271328.08kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-170.935, 0.0, 33.664),
                    'Pos': Point3(-14.863, 40.058, 6.931),
                    'Scale': VBase3(1.2849, 1.2849, 1.2849),
                    'Visual': {
                        'Color': (0.43000000715255737, 0.34999999403953552, 0.40999999642372131, 1.0),
                        'Model': 'models/props/beerstein' } },
                '1165271359.02kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-170.935, 0.0, 38.68),
                    'Pos': Point3(-16.943, 40.5188, 6.7640),
                    'Scale': VBase3(1.2849, 1.2849, 1.2849),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.69999998807907104, 0.69999998807907104, 1.0),
                        'Model': 'models/props/beerstein' } },
                '1165271411.02kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(37.07, 0.0, 0.0),
                    'Pos': Point3(-24.078, 40.270, 7.5258),
                    'Scale': VBase3(1.292, 1.292, 1.292),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165271436.39kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-110.262, 0.0, 0.0),
                    'Pos': Point3(-25.305, 40.258, 7.5049),
                    'Scale': VBase3(1.236, 1.236, 1.236),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165271581.41kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-28.524, 32.551, 4.6059),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/candle' } },
                '1165271600.35kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-5.133, 32.631, 4.6059),
                    'Scale': VBase3(0.989, 0.989, 0.989),
                    'Visual': {
                        'Model': 'models/props/candle' } },
                '1165271626.24kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(12.151, 33.1348, 4.6059),
                    'Scale': VBase3(0.78200, 0.78200, 0.78200),
                    'Visual': {
                        'Model': 'models/props/candle' } },
                '1165271663.45kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': VBase3(-99.0160, 0.0, 0.0),
                    'Pos': Point3(21.305, 3.398, 8.673),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/lamp_candle' } },
                '1165271702.97kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': VBase3(-142.668, 0.0, 0.0),
                    'Pos': Point3(1.53, 2.1840, 12.49),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/chandelier_jail' } },
                '1165271705.67kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-29.157, 14.32, 14.109),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/chandelier_jail' } },
                '1165272003.78kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, -4.586),
                    'Pos': Point3(-33.709, 32.149, 9.3510000000000009),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165272035.33kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, -4.586),
                    'Pos': Point3(-31.803, 31.341, 9.952),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/bottle_green' } },
                '1165272160.13kmuller': {
                    'Type': 'Food',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-23.667, 32.701, 6.3890),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.79000002145767212, 0.60000002384185791, 1.0),
                        'Model': 'models/props/garlicString' } },
                '1165272166.24kmuller': {
                    'Type': 'Food',
                    'Hpr': VBase3(-37.810, 0.0, 0.0),
                    'Pos': Point3(-30.216, 35.5068, 5.1539),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/ham' } },
                '1165272219.58kmuller': {
                    'Type': 'Food',
                    'Hpr': VBase3(0.0, 0.0, 1.118),
                    'Pos': Point3(-28.719, 38.198, 5.83),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.30000001192092896, 0.30000001192092896, 0.30000001192092896, 1.0),
                        'Model': 'models/props/sausage' } },
                '1165272967.63kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(53.673, 0.0, 0.0),
                    'Pos': Point3(6.3920, -4.588, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.54901963472366333, 0.62352943420410156, 0.52941179275512695, 1.0),
                        'Model': 'models/props/stool_bar' } },
                '1165272984.3kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(97.8344, 0.0, 0.0),
                    'Pos': Point3(-3.28, -5.34700, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.54901963472366333, 0.62352943420410156, 0.52941179275512695, 1.0),
                        'Model': 'models/props/chair_bar' } },
                '1165272995.24kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(-12.615, 0.0, 0.0),
                    'Pos': Point3(4.5750, 5.168, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.54901963472366333, 0.62352943420410156, 0.52941179275512695, 1.0),
                        'Model': 'models/props/chair_bar' } },
                '1165273011.24kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(1.985, -16.167, 4.147),
                    'Pos': Point3(-5.9279, 8.6383, 1.079),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.54901963472366333, 0.62352943420410156, 0.52941179275512695, 1.0),
                        'Model': 'models/props/chair_bar' } },
                '1165273214.85kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(17.109, 0.0, 0.0),
                    'Objects': { },
                    'Pos': Point3(-1.8939, 30.352, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/stool_bar_tall' } },
                '1165273221.42kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(1.7849, 0.0, 0.0),
                    'Pos': Point3(-6.689, 29.581, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.67058825492858887, 0.67058825492858887, 0.67058825492858887, 1.0),
                        'Model': 'models/props/stool_bar_tall' } },
                '1165273270.28kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(71.677, 0.0, 0.0),
                    'Pos': Point3(-32.493, 36.607, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.67058825492858887, 0.67058825492858887, 0.67058825492858887, 1.0),
                        'Model': 'models/props/stool_bar_tall' } },
                '1165273300.45kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(91.706, 0.0, 0.0),
                    'Pos': Point3(-51.886, 32.795, 1.026),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/table_bar_square' } },
                '1165273348.77kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(-53.676, 0.0, 0.0),
                    'Pos': Point3(-52.064, 28.341, 1.15),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/stool_bar' } },
                '1165273398.75kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(87.6140, 0.0, 0.0),
                    'Pos': Point3(-58.326, 17.6118, 1.113),
                    'Scale': VBase3(0.763, 0.763, 0.763),
                    'Visual': {
                        'Color': (0.85000002384185791, 0.81999999284744263, 0.73000001907348633, 1.0),
                        'Model': 'models/props/barrel_group_1' } },
                '1165273535.11kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': VBase3(-40.737, 0.0, 0.0),
                    'Objects': {
                        '1165277930.75kmuller': {
                            'Type': 'Jugs_and_Jars',
                            'Hpr': VBase3(39.838, 0.0, 0.0),
                            'Pos': Point3(-0.137, 0.303, 8.0449),
                            'Scale': VBase3(0.858, 0.858, 0.858),
                            'Visual': {
                                'Color': (0.6600000262260437, 0.56000000238418579, 0.55000001192092896, 1.0),
                                'Model': 'models/props/bottle_red' } } },
                    'Pos': Point3(-54.133, 20.916, 1.0),
                    'Scale': VBase3(1.1639, 1.1639, 1.1639),
                    'Visual': {
                        'Color': (0.40000000596046448, 0.40000000596046448, 0.40000000596046448, 1.0),
                        'Model': 'models/props/crate' } },
                '1165273600.94kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(-26.053, 0.0, 0.0),
                    'Pos': Point3(-53.978, 17.263, 1.062),
                    'Scale': VBase3(0.594, 0.594, 0.594),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel' } },
                '1165273684.05kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': VBase3(-18.9868, 0.0, 0.0),
                    'Objects': { },
                    'Pos': Point3(-54.651, 13.18, 1.0),
                    'Scale': VBase3(0.78100, 0.78100, 0.78100),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.73000001907348633, 0.57999998331069946, 1.0),
                        'Model': 'models/props/crate_04' } },
                '1165273741.35kmuller': {
                    'Type': 'Prop_Groups',
                    'DisableCollision': False,
                    'Hpr': VBase3(-37.441, 0.0, 0.0),
                    'Pos': Point3(-59.113, 0.024, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/prop_group_A' } },
                '1165273820.41kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Objects': {
                        '1165278536.22kmuller': {
                            'Type': 'Jugs_and_Jars',
                            'Hpr': VBase3(0.0, 55.649, 0.0),
                            'Pos': Point3(-1.5529, 0.35598, 8.999),
                            'Scale': VBase3(1.0, 1.0, 1.0),
                            'Visual': {
                                'Color': (0.60000002384185791, 0.60000002384185791, 0.60000002384185791, 1.0),
                                'Model': 'models/props/pitcher_brown' } } },
                    'Pos': Point3(-56.764, -1.8149, 0.993),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (1.0, 0.95999997854232788, 0.75, 1.0),
                        'Model': 'models/props/crates_group_2' } },
                '1165273860.75kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(32.095, 0.0, 0.0),
                    'Pos': Point3(-60.6438, -5.8440, 1.242),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.73000001907348633, 0.57999998331069946, 1.0),
                        'Model': 'models/props/barrel_worn' } },
                '1165273910.55kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': VBase3(180.0, -88.674, -180.0),
                    'Pos': Point3(-55.575, -8.40000, 3.89),
                    'Scale': VBase3(1.3879, 1.3879, 1.3879),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/crate_04' } },
                '1165273998.72kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(155.16, 0.0, 0.0),
                    'Pos': Point3(-46.334, -11.342, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/chair_bar' } },
                '1165274008.95kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(55.35, 0.0, 0.0),
                    'Pos': Point3(-45.887, -0.449, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/chair_bar' } },
                '1165274017.97kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(-56.822, 0.0, 0.0),
                    'Pos': Point3(-37.901, -4.58900, 1.381),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/chair_bar' } },
                '1165274050.94kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': VBase3(87.561, 0.0, 0.0),
                    'Pos': Point3(-55.087, -12.502, 8.3810),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/lamp_candle' } },
                '1165274102.6kmuller': {
                    'Type': 'Furniture',
                    'DisableCollision': False,
                    'Hpr': VBase3(-16.523, 0.0, 0.0),
                    'Pos': Point3(6.58, -21.577, 1.0),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/bench' } },
                '1165274126.86kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': VBase3(0.0, 0.0, 0.001),
                    'Pos': Point3(22.138, 10.212, 3.9208),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/candle' } },
                '1165274146.16kmuller': {
                    'Type': 'Cups',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-1.5009, -2.374, 3.956),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165274148.94kmuller': {
                    'Type': 'Cups',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(20.693, 9.457, 3.8928),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165274166.8kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-95.2510, -58.186, -96.171),
                    'Pos': Point3(-3.206, -1.5169, 4.173),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165274656.42kmuller': {
                    'Type': 'Sack',
                    'DisableCollision': False,
                    'Hpr': VBase3(-3.491, 3.37, 52.482),
                    'Pos': Point3(-55.052, -18.738, 2.8238),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.80000001192092896, 0.80000001192092896, 1.0),
                        'Model': 'models/props/Sack' } },
                '1165274726.1kmuller': {
                    'Type': 'Sack',
                    'DisableCollision': False,
                    'Hpr': VBase3(-84.037, -50.399, 10.433),
                    'Pos': Point3(-55.816, -16.637, 1.6579),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.69999998807907104, 0.69999998807907104, 1.0),
                        'Model': 'models/props/Sack' } },
                '1165274773.6kmuller': {
                    'Type': 'Sack',
                    'DisableCollision': False,
                    'Hpr': VBase3(-64.156, 0.0, 0.0),
                    'Pos': Point3(-50.774, -16.8908, 0.960),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/Sack' } },
                '1165274848.8kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.185, 0.0),
                    'Pos': Point3(11.335, 43.701, 4.66),
                    'Scale': VBase3(0.72698, 0.72698, 0.72698),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel_sideways' } },
                '1165274894.11kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(16.8908, 41.828, 4.6286),
                    'Scale': VBase3(1.212, 1.212, 1.212),
                    'Visual': {
                        'Model': 'models/props/candle' } },
                '1165274931.35kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.185, 0.0),
                    'Pos': Point3(14.911, 43.356, 4.6059),
                    'Scale': VBase3(0.893, 0.893, 0.893),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel_worn' } },
                '1165274976.27kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(2.515, 42.558, 4.54900),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.44999998807907104, 0.55000001192092896, 0.44999998807907104, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165275023.81kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.185, 0.0),
                    'Pos': Point3(-0.651, 41.551, 4.633),
                    'Scale': VBase3(0.78000, 0.78000, 0.78000),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/barrel_worn' } },
                '1165275062.16kmuller': {
                    'Type': 'Cups',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(0.998, 40.515, 4.6100),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/beerstein' } },
                '1165275084.2kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(3.879, 42.414, 4.59700),
                    'Scale': VBase3(1.365, 1.365, 1.365),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165275123.75kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(4.6668, 42.554, 4.6059),
                    'Scale': VBase3(0.836, 0.836, 0.836),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.79000002145767212, 0.60000002384185791, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275138.58kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(5.112, 42.680, 4.6059),
                    'Scale': VBase3(0.69195, 0.69195, 0.69195),
                    'Visual': {
                        'Color': (0.5899999737739563, 0.52999997138977051, 0.44999998807907104, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275160.58kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(1.516, -0.636, 3.956),
                    'Scale': VBase3(0.70796, 0.70796, 0.70796),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165275186.05kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(8.9075, 41.951, 4.6059),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165275192.22kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(-62.795, 0.0, 0.0),
                    'Pos': Point3(5.88, 41.712, 4.6059),
                    'Scale': VBase3(1.78, 1.78, 1.78),
                    'Visual': {
                        'Model': 'models/props/winebottle_B' } },
                '1165275274.69kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(-23.802, 0.0, 0.0),
                    'Pos': Point3(7.4346, 42.683, 4.6059),
                    'Scale': VBase3(1.524, 1.524, 1.524),
                    'Visual': {
                        'Model': 'models/props/waterpitcher' } },
                '1165275305.1kmuller': {
                    'Type': 'Crate',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-5.128, 40.817, 4.572),
                    'Scale': VBase3(0.799000, 0.799000, 0.799000),
                    'Visual': {
                        'Model': 'models/props/crates_group_2' } },
                '1165275369.2kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-7.2918, 41.046, 4.6630),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.43000000715255737, 0.34999999403953552, 0.34999999403953552, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165275380.81kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-7.7389, 40.094, 4.58100),
                    'Scale': VBase3(1.989, 1.989, 1.989),
                    'Visual': {
                        'Model': 'models/props/waterpitcher' } },
                '1165275418.02kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-8.999, 41.228, 4.6059),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.5899999737739563, 0.5899999737739563, 0.49000000953674316, 1.0),
                        'Model': 'models/props/largejug_B' } },
                '1165275431.75kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-9.109, 39.499, 4.615),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.5899999737739563, 0.5899999737739563, 0.49000000953674316, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275460.86kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 3.523, 0.0),
                    'Pos': Point3(-9.91, 40.161, 4.53),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.56000000238418579, 0.55000001192092896, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275483.31kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-10.727, 40.776, 4.615),
                    'Scale': VBase3(1.471, 1.471, 1.471),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165275502.77kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-13.273, 40.377, 4.56),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.44999998807907104, 0.55000001192092896, 0.44999998807907104, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165275545.97kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, -0.205, 0.0),
                    'Pos': Point3(-14.901, 39.847, 4.628),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.56000000238418579, 0.55000001192092896, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165275567.7kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.680000, -0.159, 0.002),
                    'Pos': Point3(-15.968, 39.735, 4.6286),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/bottle_brown' } },
                '1165275597.6kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.680000, -0.159, 0.002),
                    'Pos': Point3(-17.626, 40.536, 4.6070),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.72000002861022949, 0.69999998807907104, 0.5899999737739563, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165275616.55kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, 0.228),
                    'Pos': Point3(-17.152, 38.8848, 4.62),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165275673.27kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, 0.0),
                    'Pos': Point3(-19.82, 39.39, 4.6139),
                    'Scale': VBase3(1.715, 1.715, 1.715),
                    'Visual': {
                        'Model': 'models/props/jug' } },
                '1165275716.6kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-18.878, 40.332, 4.5498),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.54000002145767212, 0.4699999988079071, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275743.11kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(178.747, 1.8879, -56.420),
                    'Pos': Point3(-24.771, 39.987, 6.02500),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.56999999284744263, 0.4699999988079071, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165275789.53kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-0.451, -2.8010, 78.146),
                    'Pos': Point3(-21.936, 40.5188, 6.9088),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.50980395078659058, 0.50980395078659058, 0.50980395078659058, 1.0),
                        'Model': 'models/props/cup_tin' } },
                '1165275887.19kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-0.451, -2.8010, 81.683),
                    'Pos': Point3(-23.149, 40.076, 6.846),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.50980395078659058, 0.50980395078659058, 0.50980395078659058, 1.0),
                        'Model': 'models/props/cup_tin' } },
                '1165275933.31kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-20.562, 40.941, 7.609),
                    'Scale': VBase3(0.629, 0.629, 0.629),
                    'Visual': {
                        'Color': (0.30000001192092896, 0.30000001192092896, 0.30000001192092896, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165275976.81kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-21.148, 40.0930, 7.62),
                    'Scale': VBase3(0.629, 0.629, 0.629),
                    'Visual': {
                        'Color': (0.40000000596046448, 0.40000000596046448, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165276023.44kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-18.6, 40.421, 7.793),
                    'Scale': VBase3(0.747, 0.747, 0.747),
                    'Visual': {
                        'Color': (0.6600000262260437, 0.56000000238418579, 0.55000001192092896, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165276044.49kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(2.2040, 0.0, 0.0),
                    'Pos': Point3(-18.007, 40.110, 7.806),
                    'Scale': VBase3(0.728, 0.728, 0.728),
                    'Visual': {
                        'Color': (0.30000001192092896, 0.30000001192092896, 0.30000001192092896, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165276973.66kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-55.363, 22.8, 10.403),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.44999998807907104, 0.55000001192092896, 0.44999998807907104, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165277980.56kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(-3.423, 0.0, 0.0),
                    'Pos': Point3(-54.920, 20.908, 10.333),
                    'Scale': VBase3(0.53200, 0.53200, 0.53200),
                    'Visual': {
                        'Color': (0.75, 0.93000000715255737, 1.0, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165278031.19kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(-3.423, 0.0, 0.0),
                    'Pos': Point3(-55.40, 20.632, 10.324),
                    'Scale': VBase3(0.69895, 0.69895, 0.69895),
                    'Visual': {
                        'Color': (0.75, 0.93000000715255737, 1.0, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165278070.2kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(-109.923, 0.0, 0.0),
                    'Pos': Point3(-54.633, 19.913, 10.361),
                    'Scale': VBase3(1.343, 1.343, 1.343),
                    'Visual': {
                        'Model': 'models/props/winebottle_B' } },
                '1165278109.69kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(117.614, 0.0, 0.0),
                    'Pos': Point3(-55.478, 18.056, 10.334),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/waterpitcher' } },
                '1165278149.66kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, -0.41598),
                    'Pos': Point3(-55.645, 16.916, 10.334),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.51999998092651367, 0.51999998092651367, 0.52999997138977051, 1.0),
                        'Model': 'models/props/pitcher_brown' } },
                '1165278170.14kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-54.854, 16.359, 10.173),
                    'Scale': VBase3(0.757, 0.757, 0.757),
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165278214.22kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-55.863, 15.77, 10.166),
                    'Scale': VBase3(0.919000, 0.919000, 0.919000),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165278287.33kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-56.296, 15.068, 10.175),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.5899999737739563, 0.52999997138977051, 0.44999998807907104, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165278315.5kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-56.2688, 13.525, 10.074),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_brown' } },
                '1165278362.8kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(-107.134, 0.0, 0.0),
                    'Pos': Point3(-56.65, 7.1139, 9.941),
                    'Scale': VBase3(1.223, 1.223, 1.223),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.69999998807907104, 0.69999998807907104, 1.0),
                        'Model': 'models/props/beerstein' } },
                '1165278396.64kmuller': {
                    'Type': 'Cups',
                    'Hpr': VBase3(144.652, 0.0, 0.0),
                    'Pos': Point3(-56.582, 6.327, 9.913),
                    'Scale': VBase3(1.223, 1.223, 1.223),
                    'Visual': {
                        'Color': (0.60000002384185791, 0.72000002861022949, 0.60000002384185791, 1.0),
                        'Model': 'models/props/beerstein' } },
                '1165278441.47kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(171.096, 0.0, 0.0),
                    'Pos': Point3(-56.350, 4.181, 9.8688),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1.0),
                        'Model': 'models/props/bottle_tan' } },
                '1165278486.0kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-56.411, 3.1539, 9.8190000000000008),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.49000000953674316, 0.47999998927116394, 0.40000000596046448, 1.0),
                        'Model': 'models/props/bottle_red' } },
                '1165278512.41kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-58.689, 1.149, 9.70100),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.70999997854232788, 0.67000001668930054, 0.60000002384185791, 1.0),
                        'Model': 'models/props/bottle_brown' } },
                '1165278611.91kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-58.317, -3.899, 9.7215),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.69999998807907104, 0.69999998807907104, 0.69999998807907104, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1165278631.7kmuller': {
                    'Type': 'Jugs_and_Jars',
                    'Hpr': VBase3(0.0, 0.0, 0.0),
                    'Pos': Point3(-58.459, -5.3879, 9.6575),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1.0),
                        'Model': 'models/props/bottle_green' } },
                '1166125378.51kmuller': {
                    'Type': 'Light_Fixtures',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(0.899, -1.326, 3.956),
                    'Scale': VBase3(1.2709, 1.2709, 1.2709),
                    'Visual': {
                        'Model': 'models/props/lamp_table_hurricane_oil' } },
                '1166125484.09kmuller': {
                    'Type': 'Barrel',
                    'DisableCollision': False,
                    'Hpr': VBase3(32.095, 0.0, 0.0),
                    'Pos': Point3(-54.060, 0.320, 0.97498),
                    'Scale': VBase3(0.773, 0.773, 0.773),
                    'Visual': {
                        'Color': (0.60000002384185791, 0.60000002384185791, 0.60000002384185791, 1.0),
                        'Model': 'models/props/barrel_grey' } },
                '1166126336.53kmuller': {
                    'Type': 'Food',
                    'Hpr': VBase3(0.0, 0.0, 1.118),
                    'Pos': Point3(-7.078, 32.045, 7.8040),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.79000002145767212, 0.60000002384185791, 1.0),
                        'Model': 'models/props/sausage' } },
                '1171238953.92MAsaduzz': {
                    'Type': 'Townsperson',
                    'Category': 'Cast',
                    'AnimSet': 'tut_dan_idle',
                    'CustomModel': 'None',
                    'Hpr': VBase3(180.0, 0.0, 0.0),
                    'Patrol Radius': 12,
                    'Pos': Point3(1.5, 34.837, 1.082),
                    'PoseAnim': '',
                    'PoseFrame': '',
                    'Respawns': True,
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Start State': 'Idle',
                    'Team': 'Player' },
                '1174958597.14dzlu': {
                    'Type': 'Light - Dynamic',
                    'Attenuation': '0.005',
                    'ConeAngle': '38.8636',
                    'DropOff': '4.0909',
                    'FlickRate': 0.5,
                    'Flickering': False,
                    'Hpr': VBase3(-7.125, -17.966, -2.814),
                    'Intensity': '1.0000',
                    'LightType': 'SPOT',
                    'Pos': Point3(6.9778, 18.882, 10.023),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/light_tool_bulb' } },
                '1174958743.41dzlu': {
                    'Type': 'Light - Dynamic',
                    'Attenuation': '0.005',
                    'ConeAngle': '60.0000',
                    'DropOff': '0.0000',
                    'FlickRate': 0.5,
                    'Flickering': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Intensity': '0.5152',
                    'LightType': 'AMBIENT',
                    'Pos': Point3(-19.632, 9.4920000000000009, 3.636),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (1, 1, 1, 1),
                        'Model': 'models/props/light_tool_bulb' } },
                '1174961658.64dzlu': {
                    'Type': 'Light - Dynamic',
                    'Attenuation': '0.005',
                    'ConeAngle': '60.0000',
                    'DropOff': '0.0000',
                    'FlickRate': 0.5,
                    'Flickering': True,
                    'Hpr': VBase3(7.49600, -81.8328, -71.15),
                    'Intensity': '0.0758',
                    'LightType': 'POINT',
                    'Pos': Point3(-29.125, 14.291, 12.776),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (0.80000001192092896, 0.80000001192092896, 1.0, 1.0),
                        'Model': 'models/props/light_tool_bulb' } },
                '1174963688.81dzlu': {
                    'Type': 'Light - Dynamic',
                    'Attenuation': '0.005',
                    'ConeAngle': '60.0000',
                    'DropOff': '0.0000',
                    'FlickRate': 0.5,
                    'Flickering': True,
                    'Hpr': VBase3(-83.0663, -8.2880, -12.746),
                    'Intensity': '0.3485',
                    'LightType': 'SPOT',
                    'Pos': Point3(-7.027, 25.879, 8.8821),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/light_tool_bulb' } },
                '1200528384.0jubutler1': {
                    'Type': 'Door Locator Node',
                    'Name': 'door_locator',
                    'Hpr': VBase3(78.799, 0.0, 0.0),
                    'Pos': Point3(19.911, -9.5294, 0.486),
                    'Scale': VBase3(1.0, 1.0, 1.0) },
                '1200528384.0jubutler2': {
                    'Type': 'Door Locator Node',
                    'Name': 'door_locator_2',
                    'Hpr': VBase3(-180.0, 0.0, 0.0),
                    'Pos': Point3(-42.9368, 40.889, 0.735),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'TargetUIDs': [
                        '1190757402.45joswilso'] } },
            'Visual': {
                'Color': (1.0, 1.0, 1.0, 1.0),
                'Model': 'models/buildings/interior_tavern_c' } } },
    'Node Links': [],
    'Layers': { },
    'ObjectIds': {
        '1121212983.08Shochet0': '["Objects"]["1121212983.08Shochet0"]',
        '1154731709.64jubutler': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1154731709.64jubutler"]',
        '1165268405.64kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268405.64kmuller"]',
        '1165268489.64kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268489.64kmuller"]',
        '1165268495.0kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268495.0kmuller"]',
        '1165268541.81kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268541.81kmuller"]',
        '1165268554.8kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268554.8kmuller"]',
        '1165268615.13kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268615.13kmuller"]',
        '1165268794.17kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165268794.17kmuller"]',
        '1165269869.89kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165269869.89kmuller"]',
        '1165270073.72kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270073.72kmuller"]',
        '1165270537.52kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270537.52kmuller"]',
        '1165270634.13kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270634.13kmuller"]',
        '1165270678.5kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270678.5kmuller"]',
        '1165270699.58kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270699.58kmuller"]',
        '1165270724.27kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270724.27kmuller"]',
        '1165270820.19kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270820.19kmuller"]',
        '1165270875.49kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270875.49kmuller"]',
        '1165270982.7kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165270982.7kmuller"]',
        '1165271086.06kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271086.06kmuller"]',
        '1165271282.78kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271282.78kmuller"]',
        '1165271328.08kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271328.08kmuller"]',
        '1165271359.02kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271359.02kmuller"]',
        '1165271411.02kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271411.02kmuller"]',
        '1165271436.39kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271436.39kmuller"]',
        '1165271581.41kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271581.41kmuller"]',
        '1165271600.35kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271600.35kmuller"]',
        '1165271626.24kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271626.24kmuller"]',
        '1165271663.45kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271663.45kmuller"]',
        '1165271702.97kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271702.97kmuller"]',
        '1165271705.67kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165271705.67kmuller"]',
        '1165272003.78kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272003.78kmuller"]',
        '1165272035.33kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272035.33kmuller"]',
        '1165272160.13kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272160.13kmuller"]',
        '1165272166.24kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272166.24kmuller"]',
        '1165272219.58kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272219.58kmuller"]',
        '1165272967.63kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272967.63kmuller"]',
        '1165272984.3kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272984.3kmuller"]',
        '1165272995.24kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165272995.24kmuller"]',
        '1165273011.24kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273011.24kmuller"]',
        '1165273214.85kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273214.85kmuller"]',
        '1165273221.42kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273221.42kmuller"]',
        '1165273270.28kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273270.28kmuller"]',
        '1165273300.45kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273300.45kmuller"]',
        '1165273348.77kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273348.77kmuller"]',
        '1165273398.75kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273398.75kmuller"]',
        '1165273535.11kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273535.11kmuller"]',
        '1165273600.94kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273600.94kmuller"]',
        '1165273684.05kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273684.05kmuller"]',
        '1165273741.35kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273741.35kmuller"]',
        '1165273820.41kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273820.41kmuller"]',
        '1165273860.75kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273860.75kmuller"]',
        '1165273910.55kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273910.55kmuller"]',
        '1165273998.72kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273998.72kmuller"]',
        '1165274008.95kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274008.95kmuller"]',
        '1165274017.97kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274017.97kmuller"]',
        '1165274050.94kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274050.94kmuller"]',
        '1165274102.6kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274102.6kmuller"]',
        '1165274126.86kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274126.86kmuller"]',
        '1165274146.16kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274146.16kmuller"]',
        '1165274148.94kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274148.94kmuller"]',
        '1165274166.8kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274166.8kmuller"]',
        '1165274656.42kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274656.42kmuller"]',
        '1165274726.1kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274726.1kmuller"]',
        '1165274773.6kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274773.6kmuller"]',
        '1165274848.8kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274848.8kmuller"]',
        '1165274894.11kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274894.11kmuller"]',
        '1165274931.35kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274931.35kmuller"]',
        '1165274976.27kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165274976.27kmuller"]',
        '1165275023.81kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275023.81kmuller"]',
        '1165275062.16kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275062.16kmuller"]',
        '1165275084.2kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275084.2kmuller"]',
        '1165275123.75kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275123.75kmuller"]',
        '1165275138.58kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275138.58kmuller"]',
        '1165275160.58kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275160.58kmuller"]',
        '1165275186.05kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275186.05kmuller"]',
        '1165275192.22kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275192.22kmuller"]',
        '1165275274.69kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275274.69kmuller"]',
        '1165275305.1kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275305.1kmuller"]',
        '1165275369.2kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275369.2kmuller"]',
        '1165275380.81kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275380.81kmuller"]',
        '1165275418.02kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275418.02kmuller"]',
        '1165275431.75kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275431.75kmuller"]',
        '1165275460.86kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275460.86kmuller"]',
        '1165275483.31kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275483.31kmuller"]',
        '1165275502.77kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275502.77kmuller"]',
        '1165275545.97kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275545.97kmuller"]',
        '1165275567.7kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275567.7kmuller"]',
        '1165275597.6kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275597.6kmuller"]',
        '1165275616.55kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275616.55kmuller"]',
        '1165275673.27kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275673.27kmuller"]',
        '1165275716.6kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275716.6kmuller"]',
        '1165275743.11kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275743.11kmuller"]',
        '1165275789.53kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275789.53kmuller"]',
        '1165275887.19kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275887.19kmuller"]',
        '1165275933.31kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275933.31kmuller"]',
        '1165275976.81kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165275976.81kmuller"]',
        '1165276023.44kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165276023.44kmuller"]',
        '1165276044.49kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165276044.49kmuller"]',
        '1165276973.66kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165276973.66kmuller"]',
        '1165277930.75kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273535.11kmuller"]["Objects"]["1165277930.75kmuller"]',
        '1165277980.56kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165277980.56kmuller"]',
        '1165278031.19kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278031.19kmuller"]',
        '1165278070.2kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278070.2kmuller"]',
        '1165278109.69kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278109.69kmuller"]',
        '1165278149.66kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278149.66kmuller"]',
        '1165278170.14kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278170.14kmuller"]',
        '1165278214.22kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278214.22kmuller"]',
        '1165278287.33kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278287.33kmuller"]',
        '1165278315.5kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278315.5kmuller"]',
        '1165278362.8kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278362.8kmuller"]',
        '1165278396.64kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278396.64kmuller"]',
        '1165278441.47kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278441.47kmuller"]',
        '1165278486.0kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278486.0kmuller"]',
        '1165278512.41kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278512.41kmuller"]',
        '1165278536.22kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165273820.41kmuller"]["Objects"]["1165278536.22kmuller"]',
        '1165278611.91kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278611.91kmuller"]',
        '1165278631.7kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1165278631.7kmuller"]',
        '1166125378.51kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1166125378.51kmuller"]',
        '1166125484.09kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1166125484.09kmuller"]',
        '1166126336.53kmuller': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1166126336.53kmuller"]',
        '1171238953.92MAsaduzz': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1171238953.92MAsaduzz"]',
        '1174958597.14dzlu': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1174958597.14dzlu"]',
        '1174958743.41dzlu': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1174958743.41dzlu"]',
        '1174961658.64dzlu': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1174961658.64dzlu"]',
        '1174963688.81dzlu': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1174963688.81dzlu"]',
        '1200528384.0jubutler1': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1200528384.0jubutler1"]',
        '1200528384.0jubutler2': '["Objects"]["1121212983.08Shochet0"]["Objects"]["1200528384.0jubutler2"]' } }
extraInfo = {
    'camPos': Point3(-85.9330, 83.2948, 33.1736),
    'camHpr': VBase3(-141, -21, 0),
    'focalLength': 1.3999999761599999 }
