from pandac.PandaModules import Point3, VBase3, Vec4, Vec3
objectStruct = {
    'LevelEnvironment': {
        'Dawn': {
            'AmbientColor': Vec4(0.22, 0.348, 0.540000, 1),
            'BackColor': Vec4(0.288, 0.540000, 0.390, 1),
            'Direction': Vec3(0.0, 35.0, 210.0),
            'FogColor': Vec4(0.27, 0.44, 0.450, 0),
            'FogExp': 0.00300,
            'FrontColor': Vec4(1.51, 1.5, 1.01, 1),
            'SkyType': 10 },
        'Day': {
            'AmbientColor': Vec4(0.670000, 0.848, 0.91000, 1),
            'BackColor': Vec4(0.239, 0.489, 0.418, 1),
            'FogColor': Vec4(0.598, 0.696, 0.9, 0),
            'FogExp': 0.00060000002849800002,
            'FrontColor': Vec4(2, 1.79, 1.67, 1),
            'SkyType': 10 },
        'Dusk': {
            'AmbientColor': Vec4(0.38, 0.299, 0.44, 1),
            'BackColor': Vec4(0.489, 0.66000, 0.418, 1),
            'Direction': Vec3(0.0, 35.0, 15.0),
            'FogColor': Vec4(0.359, 0.288, 0.12, 0),
            'FogExp': 0.002,
            'FrontColor': Vec4(1.73, 1.04, 0.930000, 1),
            'SkyType': 10 },
        'Night': {
            'AmbientColor': Vec4(0.37, 0.62, 0.680000, 1),
            'BackColor': Vec4(0.5, 0.920000, 1.41, 1),
            'Direction': Vec3(0.0, 40.0, 90.0),
            'FogColor': Vec4(0, 0.070, 0.070, 0),
            'FogExp': 0.0020000000949899998,
            'FrontColor': Vec4(0.260, 0.609, 0.478, 1),
            'SkyType': 11 },
        'Stars': {
            'AmbientColor': Vec4(0.25, 0.58, 0.58, 1),
            'BackColor': Vec4(0.946, 0.946, 1.14, 1),
            'Direction': Vec3(0.0, 40.0, 160.0),
            'FogColor': Vec4(0, 0, 0, 0),
            'FogExp': 0.00079999997979000002,
            'FrontColor': Vec4(0.260, 0.489, 0.560000, 1),
            'SkyType': 11 } },
    'Objects': {
        '1170793088.0jubutler': {
            'Type': 'Island Game Area',
            'Name': 'pvp_deathmatchArea1_jungle_a',
            'File': '',
            'Environment': 'Jungle',
            'Footstep Sound': 'Sand',
            'Instanced': True,
            'Minimap': True,
            'Minimap Prefix': 'minimap_PillagersPass',
            'Objects': {
                '1170793216.0jubutler': {
                    'Type': 'Locator Node',
                    'Name': 'portal_interior_1',
                    'Hpr': VBase3(-81.0, 0.0, 0.0),
                    'Pos': Point3(36.719, 255.714, 7.0596),
                    'Scale': VBase3(1.0, 1.0, 1.0) },
                '1170793216.0jubutler0': {
                    'Type': 'Locator Node',
                    'Name': 'portal_interior_2',
                    'Hpr': VBase3(142.378, 0.0, 0.0),
                    'Pos': Point3(837.182, 5.1668, 52.393),
                    'Scale': VBase3(1.0, 1.0, 1.0) },
                '1170793216.0jubutler1': {
                    'Type': 'Player Spawn Node',
                    'Hpr': VBase3(166.334, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(729.955, 166.654, 53.235),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 1',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913647.47darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': VBase3(148.143, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(834.693, 152.574, 53.0008),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 1',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913737.7darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': Point3(90.0, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(734.226, -6.5418, 53.039),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 1',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913749.09darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': VBase3(22.891, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(803.602, 64.1913, 53.371),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 1',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913790.58darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': Point3(-90.0, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(-62.151, 92.8850, 9.8594),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 2',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913808.86darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': VBase3(42.3788, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(182.045, -10.92, 37.219),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 2',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913819.52darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': VBase3(-34.792, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(21.867, -66.527, 0.389),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 2',
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170913839.28darren': {
                    'Type': 'Player Spawn Node',
                    'Hpr': Point3(-90.0, 0.0, 0.0),
                    'Index': -1,
                    'Pos': Point3(15.869, 164.216, 8.6544),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Spawnables': 'Team 2',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1),
                        'Model': 'models/misc/smiley' } },
                '1170978980.97darren': {
                    'Type': 'Locator Node',
                    'Name': 'portal_interior_3',
                    'Hpr': VBase3(-79.7360, 0.0, 0.0),
                    'Pos': Point3(380.725, 407.485, 61.219),
                    'Scale': VBase3(1.0, 1.0, 1.0) },
                '1171402341.13darren': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': False,
                    'Hpr': VBase3(26.169, 0.0, 0.0),
                    'Pos': Point3(11.071, 239.119, 3.5510),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1171402396.28darren': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-30.042, 0.0, 0.0),
                    'Pos': Point3(427.38, 365.223, 57.887),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1171402484.72darren': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': False,
                    'Hpr': VBase3(-122.275, 0.0, 0.0),
                    'Pos': Point3(842.811, 0.264, 51.965),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1182814104.67kmuller': {
                    'Type': 'Rock',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-71.783, 0.0, 0.0),
                    'Pos': Point3(345.541, 111.68, 55.305),
                    'Scale': VBase3(3.41, 3.41, 3.41),
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.40000000596046448, 0.40000000596046448, 0.40000000596046448, 1.0),
                        'Model': 'models/props/rock_3_sphere' } },
                '1182878962.0kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(236.668, 304.855, 50.520),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_f' } },
                '1182879030.06kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(163.686, 288.747, 38.603),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_h' } },
                '1182879074.77kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(180.604, 269.064, 45.270),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182879141.03kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-50.0188, -82.7, -5.97200),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_g' } },
                '1182879273.41kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': VBase3(-93.438, 0.0, 0.0),
                    'Pos': Point3(-53.042, 172.883, 10.917),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/fern_tree_a' } },
                '1182879325.2kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-3.08, 0.0, 14.005),
                    'Pos': Point3(-49.969, 177.366, 10.845),
                    'Scale': VBase3(1.437, 1.437, 1.437),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182879369.95kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(8.891, 0.0, 0.0),
                    'Pos': Point3(-63.4978, 175.586, 11.521),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1182879410.14kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(139.532, 0.0, 0.0),
                    'Pos': Point3(-60.097, 171.983, 11.162),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_c' } },
                '1182880772.45kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(745.655, -39.5678, 51.813),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_tree_a' } },
                '1182881679.11kmuller': {
                    'Type': 'Vines',
                    'DisableCollision': False,
                    'Hpr': VBase3(17.763, 0.0, 0.0),
                    'Pos': Point3(704.116, -43.125, 72.772),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_vine_a' } },
                '1182881711.8kmuller': {
                    'Type': 'Vines',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(777.607, -19.864, 75.003),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_vine_a' } },
                '1182881836.17kmuller': {
                    'Type': 'Vines',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(592.798, -22.1728, 92.1320),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_vine_a' } },
                '1182881912.22kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(17.949, 0.0, 0.0),
                    'Pos': Point3(462.399, 294.492, 64.811),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182881939.48kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(121.346, 0.0, 0.0),
                    'Pos': Point3(455.966, 294.759, 64.704),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/fern_tree_a' } },
                '1182882005.38kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(69.810, 0.0, 0.0),
                    'Pos': Point3(457.341, 301.848, 64.372),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182882019.13kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(91.334, 0.0, 0.0),
                    'Pos': Point3(447.685, 302.560, 64.0738),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182882095.56kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(82.363, 0.0, 0.0),
                    'Pos': Point3(396.295, 221.798, 60.475),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182882150.58kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -18.378, 0.0),
                    'Pos': Point3(389.822, 226.782, 59.927),
                    'Scale': VBase3(1.151, 1.151, 1.151),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182882170.84kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -16.9868, 19.0538),
                    'Pos': Point3(414.992, 222.13, 62.091),
                    'Scale': VBase3(1.139, 1.139, 1.139),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182882199.61kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(70.287, -3.6388, 17.382),
                    'Pos': Point3(408.83, 225.756, 61.557),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182882218.36kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(416.439, 230.866, 62.573),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182882232.19kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, 13.532),
                    'Pos': Point3(428.415, 217.223, 63.546),
                    'Scale': VBase3(0.771, 0.771, 0.771),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182882314.53kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Hpr': VBase3(-39.716, 0.0, 0.0),
                    'Pos': Point3(689.61, 234.372, 37.9578),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_tree_a' } },
                '1182882445.39kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(6.5830, 369.165, 27.603),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_e' } },
                '1182882526.47kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(123.232, 397.226, 2.0489),
                    'Scale': VBase3(1.27, 1.27, 1.27),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_d' } },
                '1182882605.27kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(225.286, -14.279, 50.293),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_b' } },
                '1182882633.61kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-2.0609, 0.0, 0.0),
                    'Pos': Point3(222.057, -2.9649, 45.837),
                    'Scale': VBase3(1.205, 1.205, 1.205),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182882820.56kmuller': {
                    'Type': 'Collision Barrier',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(795.975, 178.222, 51.981),
                    'Scale': VBase3(4.4580, 4.4580, 4.4580),
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_sphere' } },
                '1182882887.55kmuller': {
                    'Type': 'Vines',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(760.557, 184.666, 71.29),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_vine_a' } },
                '1182882922.02kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(779.40, 168.173, 51.7658),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182882948.95kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(0.0, 0.0, -14.843),
                    'Pos': Point3(774.380, 179.072, 54.25),
                    'Scale': VBase3(1.1459, 1.1459, 1.1459),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182883004.3kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': VBase3(-103.134, 16.811, 3.2160),
                    'Pos': Point3(382.387, 229.186, 57.959),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/fern_tree_d' } },
                '1182883034.56kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(420.706, 220.758, 62.588),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/fern_tree_b' } },
                '1182883082.84kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(200.84, -42.970, 42.1958),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182883114.56kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(217.645, -18.724, 45.654),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182883544.25kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(481.310, 146.919, 73.3404),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_c' } },
                '1182883780.25kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, 0.212),
                    'Pos': Point3(39.095, 102.837, 12.859),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_h' } },
                '1182893238.66kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': VBase3(36.351, 0.0, 0.0),
                    'Pos': Point3(484.990, 123.013, 68.8856),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1182893279.69kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': VBase3(13.295, 0.0, 0.0),
                    'Pos': Point3(489.783, 141.443, 68.3178),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182893305.72kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': VBase3(-33.396, 0.0, 0.0),
                    'Pos': Point3(494.694, 119.527, 68.278),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182893330.14kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(485.333, 108.861, 68.341),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1182893343.03kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': VBase3(-25.39, 0.0, 0.0),
                    'Pos': Point3(479.665, 115.193, 67.5663),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182893391.86kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': VBase3(14.194, 0.0, 0.0),
                    'Pos': Point3(477.151, 123.384, 67.2214),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182893473.22kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': VBase3(-81.9805, 0.0, 0.0),
                    'Pos': Point3(284.757, 239.848, 48.984),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_f' } },
                '1182893504.52kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(280.512, 217.484, 49.520),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1182893516.55kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(284.166, 208.519, 50.905),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182893528.22kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(37.938, 0.0, 0.0),
                    'Pos': Point3(276.12, 206.525, 49.433),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182893666.75kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(-44.829, 0.0, 0.0),
                    'Pos': Point3(283.848, 194.811, 49.7),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_g' } },
                '1182893701.58kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, -3.41),
                    'Pos': Point3(277.625, 196.494, 49.728),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182893745.98kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, -8.394),
                    'Pos': Point3(272.809, 215.514, 49.6258),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182893812.02kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(289.68, 207.327, 47.725),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/gen_tree_a' } },
                '1182893835.89kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.810000, 2.7749),
                    'Pos': Point3(289.454, 204.09, 50.810),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182893853.69kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(287.663, 210.69, 50.683),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182894165.75kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(2.4159, -12.558, 10.983),
                    'Pos': Point3(-27.401, -37.738, 7.96),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182894212.08kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(-116.766, 0.0, -14.807),
                    'Pos': Point3(-29.109, -32.677, 1.361),
                    'Scale': VBase3(0.796000, 0.796000, 0.796000),
                    'Visual': {
                        'Model': 'models/vegetation/bush_leaves' } },
                '1182894264.47kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(-155.925, -6.3010, -18.388),
                    'Pos': Point3(-25.559, -41.911, 1.766),
                    'Scale': VBase3(0.678000, 0.678000, 0.678000),
                    'Visual': {
                        'Model': 'models/vegetation/bush_leaves' } },
                '1182894369.28kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-119.813, 0.0, -5.3866),
                    'Pos': Point3(-11.31, -70.619, 6.213),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_c' } },
                '1182894489.5kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -19.446, 14.571),
                    'Pos': Point3(4.867, -98.460, 4.402),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182894516.83kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(2.2749, -12.805, 19.382),
                    'Pos': Point3(-16.184, -77.9805, 7.827),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182894619.73kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(534.638, -99.8765, 56.951),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/jungle_tree_b' } },
                '1182894920.72kmuller': {
                    'Type': 'Tree',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(3.5578, -8.1668, -16.033),
                    'Pos': Point3(816.655, 38.088, 50.6258),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/fern_tree_d' } },
                '1182894988.89kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -8.9462, 0.0),
                    'Pos': Point3(820.962, 43.508, 52.0008),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182895013.33kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-69.340, 0.0, 0.0),
                    'Pos': Point3(813.078, 29.733, 52.219),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182895142.08kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(-33.53, 1.62, 0.0),
                    'Pos': Point3(306.343, 170.418, 53.389),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_d' } },
                '1182895181.08kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(-94.9503, 0.0, 0.0),
                    'Pos': Point3(303.596, 152.919, 52.15),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182895214.06kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(137.99, 0.0, 0.0),
                    'Pos': Point3(312.10, 177.992, 53.359),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_c' } },
                '1182895366.86kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(497.036, 125.881, 67.8404),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182895388.58kmuller': {
                    'Type': 'Collision Barrier',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(487.247, 117.797, 67.822),
                    'Scale': VBase3(3.3109, 3.3109, 3.3109),
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_sphere' } },
                '1182895463.88kmuller': {
                    'Type': 'Collision Barrier',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(476.144, 105.64, 67.563),
                    'Scale': VBase3(1.153, 1.153, 1.153),
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_sphere' } },
                '1182895495.86kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': VBase3(18.443, 0.0, 0.0),
                    'Pos': Point3(497.442, 112.316, 68.025),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182895526.56kmuller': {
                    'Type': 'Collision Barrier',
                    'DisableCollision': False,
                    'Hpr': VBase3(115.852, 0.0, 0.0),
                    'Pos': Point3(495.202, 136.642, 68.5964),
                    'Scale': VBase3(2.3109, 2.3109, 2.3109),
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1182895565.48kmuller': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(484.103, 136.042, 67.853),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/bush_b' } },
                '1182895578.3kmuller': {
                    'Type': 'Collision Barrier',
                    'DisableCollision': False,
                    'Hpr': VBase3(-117.387, 0.0, 0.0),
                    'Pos': Point3(485.779, 139.117, 67.801),
                    'Scale': VBase3(1.9119, 1.9119, 1.9119),
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1182895794.27kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(93.609, 2.548, 29.920),
                    'Pos': Point3(85.1386, -92.4865, 16.474),
                    'Scale': VBase3(1.369, 1.369, 1.369),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182895831.91kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-98.2576, 25.1788, -8.327),
                    'Pos': Point3(95.8163, -98.0633, 21.588),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Color': (1.0, 0.80000001192092896, 0.60000002384185791, 1.0),
                        'Model': 'models/vegetation/jungle_fern_c' } },
                '1182895912.91kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -19.7048, 4.4809),
                    'Pos': Point3(94.188, -94.2420, 41.5678),
                    'Scale': VBase3(1.508, 1.508, 1.508),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896654.44kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-4.84700, -18.648, -14.852),
                    'Pos': Point3(81.728, -78.4744, 2.322),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896712.94kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, -5.8528),
                    'Pos': Point3(77.7305, -83.405, 1.0649),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896730.05kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-3.709, -8.3480000000000008, -24.059),
                    'Pos': Point3(80.647, -67.7184, 1.474),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896752.06kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, -15.41, -19.145),
                    'Pos': Point3(88.241, -75.275, 5.40800),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896791.03kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-9.41900, -29.981, -18.363),
                    'Pos': Point3(88.869, -82.5015, 15.41),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182896838.22kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, -15.031),
                    'Pos': Point3(117.081, -85.662, 19.841),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1182896857.69kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(-1.6539, -6.2389, -15.086),
                    'Pos': Point3(120.165, -79.9518, 20.856),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_a' } },
                '1182896876.94kmuller': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Hpr': VBase3(0.0, 0.0, -20.19),
                    'Pos': Point3(113.218, -81.7690, 17.956),
                    'Scale': VBase3(0.616, 0.616, 0.616),
                    'Visual': {
                        'Model': 'models/vegetation/jungle_fern_b' } },
                '1296000903.4dxschafe': {
                    'Type': 'Rock',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-7.12, -39.384, -7.4359),
                    'Scale': VBase3(15.473, 15.473, 15.473),
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.5, 0.5, 0.5, 1.0),
                        'Model': 'models/props/rock_4_sphere' } },
                '1296001095.51dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-56.963, 0.0, 0.0),
                    'Pos': Point3(241.96, 50.604, 21.087),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296001209.79dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(138.708, 0.0, 0.0),
                    'Pos': Point3(270.680, 62.127, 40.0648),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296001473.01dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-83.912, 0.0, 0.0),
                    'Pos': Point3(606.798, 152.268, 42.732),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296001494.81dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(99.063, 0.0, 0.0),
                    'Pos': Point3(626.400, 143.556, 28.134),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296001873.23dxschafe': {
                    'Type': 'Rock',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(-2.6840, -20.41, -9.18500),
                    'Scale': VBase3(9.2313, 9.2313, 9.2313),
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.40000000596046448, 0.40000000596046448, 0.40000000596046448, 1.0),
                        'Model': 'models/props/rock_3_sphere' } },
                '1296001996.47dxschafe': {
                    'Type': 'Rock',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(23.8328, 22.89, -8.0830),
                    'Scale': VBase3(8.1286, 8.1286, 8.1286),
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.54000002145767212, 0.52999997138977051, 0.52999997138977051, 1.0),
                        'Model': 'models/props/rock_3_sphere' } },
                '1296002311.4dxschafe': {
                    'Type': 'Rock',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(0.645, 0.0, 0.0),
                    'Pos': Point3(34.646, 16.417, -6.0338),
                    'Scale': VBase3(16.454, 16.454, 16.454),
                    'VisSize': '',
                    'Visual': {
                        'Color': (0.54000002145767212, 0.52999997138977051, 0.52999997138977051, 1.0),
                        'Model': 'models/props/rock_4_sphere' } },
                '1296002499.26dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-40.880, 0.0, 0.0),
                    'Pos': Point3(750.173, 45.142, 48.267),
                    'Scale': VBase3(0.676000, 0.676000, 0.676000),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296002574.36dxschafe': {
                    'Type': 'Jungle_Props_large',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(114.59, 0.0, 0.0),
                    'Pos': Point3(758.940, 20.337, 43.823),
                    'Scale': VBase3(0.865, 0.865, 0.865),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/props/cliff_jungle_low' } },
                '1296003330.89dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(32.7628, 0.0, 0.0),
                    'Pos': Point3(195.300, 103.28, 68.825),
                    'Scale': VBase3(1.338, 1.338, 1.338),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1296003457.09dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(104.456, 0.0, 0.0),
                    'Pos': Point3(200.025, 93.671, 78.183),
                    'Scale': VBase3(1.334, 1.334, 1.334),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1296003522.06dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(12.323, 0.0, 0.0),
                    'Pos': Point3(718.317, 82.0814, 80.8640),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1296003753.53dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-74.447, 0.0, 0.0),
                    'Pos': Point3(623.096, 74.903, 80.4578),
                    'Scale': VBase3(1.76, 1.76, 1.76),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1296003804.4dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(617.37, 90.600, 88.581),
                    'Scale': VBase3(1.5129, 1.5129, 1.5129),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299005027.93dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(720.701, 67.5960, 36.582),
                    'Scale': VBase3(4.205, 4.205, 4.205),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_tube' } },
                '1299005184.47dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(111.578, 0.0, 0.0),
                    'Pos': Point3(794.547, 42.816, 52.142),
                    'Scale': VBase3(3.495, 4.4820, 9.4942),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005251.06dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(132.230, 0.0, 0.0),
                    'Pos': Point3(777.638, 71.1145, 52.155),
                    'Scale': VBase3(3.2349, 3.2349, 9.54400),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005299.73dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(159.752, 0.0, 0.0),
                    'Pos': Point3(752.65, 88.075, 52.1318),
                    'Scale': VBase3(3.0738, 2.1110, 9.5391),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005356.8dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(728.106, 82.009, 52.130),
                    'Scale': VBase3(3.0750, 3.0750, 3.0750),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_tube' } },
                '1299005406.16dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-56.82, 0.0, 0.0),
                    'Pos': Point3(721.272, 33.450, 52.265),
                    'Scale': VBase3(3.3919, 3.737, 8.7758),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005463.63dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-2.7629, 0.0, 0.0),
                    'Pos': Point3(639.216, 59.5828, 50.93),
                    'Scale': VBase3(2.201, 3.3570, 7.37),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005504.14dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-33.1928, 0.0, 0.0),
                    'Pos': Point3(744.546, 10.294, 52.307),
                    'Scale': VBase3(3.3519, 1.377, 8.7789),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005587.34dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-16.568, 0.0, 0.0),
                    'Pos': Point3(773.277, -2.9260, 52.225),
                    'Scale': VBase3(3.1190, 3.1190, 8.5872),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005766.98dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(76.933, 0.0, 0.0),
                    'Pos': Point3(667.73, 102.258, 52.091),
                    'Scale': VBase3(3.9288, 2.3968, 10.694),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005796.15dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(108.126, 0.0, 0.0),
                    'Pos': Point3(668.288, 132.568, 52.067),
                    'Scale': VBase3(2.411, 2.411, 11.237),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005829.18dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(40.927, 0.0, 0.0),
                    'Pos': Point3(668.331, 147.408, 52.03),
                    'Scale': VBase3(1.304, 1.304, 11.193),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005876.92dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(92.293, 0.0, 0.0),
                    'Pos': Point3(672.418, 167.544, 52.012),
                    'Scale': VBase3(3.261, 3.261, 11.19),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005929.28dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(22.536, 0.0, 0.0),
                    'Pos': Point3(628.619, 59.033, 52.887),
                    'Scale': VBase3(1.0, 1.0, 8.1032),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299005959.3dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-19.989, 0.0, 0.0),
                    'Pos': Point3(612.717, 61.213, 53.35),
                    'Scale': VBase3(2.47, 2.47, 9.16),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299006015.54dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-44.8118, 0.0, 0.0),
                    'Pos': Point3(587.075, 79.432, 55.591),
                    'Scale': VBase3(4.0700, 4.0700, 10.733),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299006066.12dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-70.515, 0.0, 0.0),
                    'Pos': Point3(564.946, 116.292, 58.896),
                    'Scale': VBase3(4.8440, 4.8440, 10.132),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299006151.94dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-99.7240, 0.0, 0.0),
                    'Pos': Point3(562.798, 172.181, 59.6566),
                    'Scale': VBase3(6.8946, 6.8946, 9.9280000000000008),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299006424.19dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': Point3(0.0, 0.0, 0.0),
                    'Pos': Point3(562.975, 152.854, 61.235),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299006444.35dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(106.436, 0.0, 0.0),
                    'Pos': Point3(567.851, 165.86, 61.408),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299006481.68dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(75.302, 0.0, 0.0),
                    'Pos': Point3(665.166, 172.556, 51.991),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299006908.84dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(59.978, 0.0, 0.0),
                    'Pos': Point3(656.394, 71.3178, 51.982),
                    'Scale': VBase3(2.822, 2.822, 7.141),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299007243.23dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(167.236, 0.0, 0.0),
                    'Pos': Point3(661.673, 93.537, 52.173),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299007328.79dxschafe': {
                    'Type': 'Jungle_Props',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(46.22, 0.0, 0.0),
                    'Pos': Point3(564.711, 125.804, 60.6),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/jungle_plant_c' } },
                '1299008023.1dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(157.162, 0.0, 0.0),
                    'Pos': Point3(727.900, 32.152, 52.2718),
                    'Scale': VBase3(0.735, 0.735, 0.735),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299008069.45dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': True,
                    'Holiday': '',
                    'Hpr': VBase3(-57.618, 0.0, 0.0),
                    'Pos': Point3(778.52, 0.81294, 52.32),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299008172.7dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(42.026, 0.0, 0.0),
                    'Pos': Point3(715.41099999999994, 42.521, 51.481),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299008387.87dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(0.859, 0.0, 0.0),
                    'Pos': Point3(395.615, 101.147, 56.244),
                    'Scale': VBase3(4.5529, 4.5529, 8.4488),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299008427.65dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(53.752, 0.0, 0.0),
                    'Pos': Point3(429.608, 116.974, 61.792),
                    'Scale': VBase3(3.9380, 3.9380, 7.9858),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299015944.45dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(58.375, 0.0, 0.0),
                    'Pos': Point3(449.408, 145.961, 65.1024),
                    'Scale': VBase3(3.1659, 3.1659, 7.431),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016041.0dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(74.182, 0.0, 0.0),
                    'Pos': Point3(460.199, 169.765, 64.6080),
                    'Scale': VBase3(2.374, 2.374, 6.665),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016096.64dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(108.037, 0.0, 0.0),
                    'Pos': Point3(460.348, 190.732, 66.210),
                    'Scale': VBase3(2.04, 2.04, 6.58),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016135.41dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(152.035, 0.0, 0.0),
                    'Pos': Point3(438.043, 210.717, 64.325),
                    'Scale': VBase3(4.4279, 4.4279, 6.6870),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016198.17dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(169.91, 0.0, 0.0),
                    'Pos': Point3(400.610, 223.611, 60.8328),
                    'Scale': VBase3(3.7028, 3.161, 7.312),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016245.59dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(166.888, 0.0, 0.0),
                    'Pos': Point3(371.476, 229.486, 57.478),
                    'Scale': VBase3(2.496, 2.496, 7.9850),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016287.23dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-162.983, 0.0, 0.0),
                    'Pos': Point3(344.752, 227.645, 56.055),
                    'Scale': VBase3(3.1179, 3.1179, 8.506),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016345.28dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-100.79, 0.0, 0.0),
                    'Pos': Point3(328.384, 212.717, 54.609),
                    'Scale': VBase3(2.1030, 2.1030, 8.2598),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016436.66dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-125.166, 0.0, 0.0),
                    'Pos': Point3(313.88, 185.003, 53.363),
                    'Scale': VBase3(4.7640, 4.7640, 7.6108),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016482.6dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-80.536, 0.0, 0.0),
                    'Pos': Point3(301.685, 159.354, 51.521),
                    'Scale': VBase3(1.213, 1.0329, 5.7616),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016541.41dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-50.551, 0.0, 0.0),
                    'Pos': Point3(310.656, 143.58, 53.529),
                    'Scale': VBase3(2.577, 2.528, 5.5756),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016586.49dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-45.7688, 0.0, 0.0),
                    'Pos': Point3(329.968, 121.98, 54.695),
                    'Scale': VBase3(3.2509, 3.1659, 8.4684),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016643.53dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-15.499, 0.0, 0.0),
                    'Pos': Point3(356.726, 105.364, 57.359),
                    'Scale': VBase3(3.394, 3.27, 8.3496),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016772.47dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(91.2034, 0.0, 0.0),
                    'Pos': Point3(327.488, 57.819, 55.200),
                    'Scale': VBase3(1.9139, 1.821, 5.90000),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016822.15dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(148.413, 0.0, 0.0),
                    'Pos': Point3(312.899, 76.2455, 52.286),
                    'Scale': VBase3(3.4048, 3.4048, 6.2670),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016871.9dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(83.081, 0.0, 0.0),
                    'Pos': Point3(300.345, 90.384, 52.183),
                    'Scale': VBase3(1.728, 1.728, 4.133),
                    'VisSize': '',
                    'Visual': {
                        'Color': (1.0, 1.0, 1.0, 1.0),
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016935.79dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(142.774, 0.0, 0.0),
                    'Pos': Point3(286.326, 110.459, 51.152),
                    'Scale': VBase3(3.8010, 3.8010, 3.8010),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299016970.52dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(168.383, 0.0, 0.0),
                    'Pos': Point3(245.098, 127.066, 45.2538),
                    'Scale': VBase3(5.343, 5.343, 5.343),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017032.85dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-158.484, 0.0, 0.0),
                    'Pos': Point3(201.354, 125.477, 36.829),
                    'Scale': VBase3(3.8069, 3.536, 6.9770),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017165.26dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-110.587, 0.0, 0.0),
                    'Pos': Point3(179.133, 106.269, 36.158),
                    'Scale': VBase3(2.6428, 2.6428, 8.2880),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017212.52dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(0.0, -0.360, 0.0),
                    'Pos': Point3(180.723, 99.177, 38.0068),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299017256.68dxschafe': {
                    'Type': 'Bush',
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-126.388, 0.0, 0.0),
                    'Pos': Point3(182.706, 111.792, 36.179),
                    'Scale': VBase3(1.0, 1.0, 1.0),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/vegetation/bush_a' } },
                '1299017293.23dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-35.258, 0.0, 0.0),
                    'Pos': Point3(183.700, 87.427, 36.622),
                    'Scale': VBase3(2.29, 2.4940, 7.53),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017345.73dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-106.968, 0.0, 0.0),
                    'Pos': Point3(189.497, 69.822, 37.411),
                    'Scale': VBase3(2.41, 2.261, 7.4328),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017391.34dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-47.9548, 0.0, 0.0),
                    'Pos': Point3(197.196, 45.90, 36.774),
                    'Scale': VBase3(3.3768, 3.3768, 7.61300),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017434.99dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-123.427, 0.0, 0.0),
                    'Pos': Point3(204.905, 28.923, 42.423),
                    'Scale': VBase3(1.233, 1.2789, 6.4660),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017467.95dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-56.052, 0.0, 0.0),
                    'Pos': Point3(205.328, 18.227, 41.982),
                    'Scale': VBase3(1.348, 1.3149, 5.9346),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017522.01dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-8.5, 0.0, 0.0),
                    'Pos': Point3(215.407, 11.596, 44.173),
                    'Scale': VBase3(1.323, 1.079, 5.2030),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017572.31dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-72.2576, 0.0, 0.0),
                    'Pos': Point3(218.170, 8.8016, 45.398),
                    'Scale': VBase3(1.0, 1.0, 5.0289),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017821.85dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-118.305, 0.0, 0.0),
                    'Pos': Point3(-15.069, -19.219, 0.0),
                    'Scale': VBase3(2.519, 3.069, 3.6828),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017857.85dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-53.789, 0.0, 0.0),
                    'Pos': Point3(-11.664, -4.4328, 0.0),
                    'Scale': VBase3(1.0, 1.0, 6.2198),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017878.48dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-101.482, 0.0, 0.0),
                    'Pos': Point3(-12.922, 7.328, 0.0),
                    'Scale': VBase3(1.633, 1.633, 6.2720),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017913.86dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-119.733, 0.0, 0.0),
                    'Pos': Point3(-9.40300, 18.535, 0.0),
                    'Scale': VBase3(0.776, 1.0, 6.2240),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299017969.0dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-163.41, 0.0, 0.0),
                    'Pos': Point3(1.875, 24.585, 0.0),
                    'Scale': VBase3(1.992, 1.0, 6.4649),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018020.76dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-150.188, 0.0, 0.0),
                    'Pos': Point3(17.48, 30.8648, -1.193),
                    'Scale': VBase3(1.488, 1.488, 3.645),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018064.79dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-178.234, 0.0, 0.0),
                    'Pos': Point3(31.446, 34.707, 0.0),
                    'Scale': VBase3(1.583, 1.583, 3.338),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018104.2dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(161.080, 0.0, 0.0),
                    'Pos': Point3(43.884, 33.3938, 0.0),
                    'Scale': VBase3(1.0, 1.0, 3.3639),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018135.89dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(99.774, 0.0, 0.0),
                    'Pos': Point3(49.268, 26.984, -0.498),
                    'Scale': VBase3(1.0, 1.0, 3.6348),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018159.23dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(74.0708, 0.0, 0.0),
                    'Pos': Point3(47.2078, 11.945, -1.038),
                    'Scale': VBase3(2.2120, 2.5578, 2.5578),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018201.33dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(1.534, 0.0, 0.0),
                    'Pos': Point3(35.462, 1.341, 0.0),
                    'Scale': VBase3(1.802, 1.802, 2.382),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018227.75dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(86.3850, 0.0, 0.0),
                    'Pos': Point3(27.108, -1.992, -0.425),
                    'Scale': VBase3(0.717, 1.0, 7.2006),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018259.9dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(42.814, 0.0, 0.0),
                    'Pos': Point3(23.702, -8.2696, 0.0),
                    'Scale': VBase3(0.892, 1.076, 7.0860),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018293.18dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(15.894, 0.0, 0.0),
                    'Pos': Point3(12.981, -13.377, -0.53600),
                    'Scale': VBase3(1.6479, 1.834, 7.1846),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018345.51dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(83.766, 0.0, 0.0),
                    'Pos': Point3(4.7510, -21.036, 0.0),
                    'Scale': VBase3(1.1439, 1.1439, 3.7028),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018375.08dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(107.675, 0.0, 0.0),
                    'Pos': Point3(5.535, -30.713, -0.334),
                    'Scale': VBase3(0.902, 1.0, 2.979),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018411.16dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(74.488, 0.0, 0.0),
                    'Pos': Point3(4.44800, -43.933, -1.2689),
                    'Scale': VBase3(1.8939, 1.8939, 2.5089),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018445.2dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(-3.7749, 0.0, 0.0),
                    'Pos': Point3(-3.0058, -52.713, -0.22),
                    'Scale': VBase3(1.0, 1.0, 2.261),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018490.56dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(73.954, 0.0, 0.0),
                    'Pos': Point3(-7.3078, -57.146, -0.545000),
                    'Scale': VBase3(1.0, 1.0, 2.326),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } },
                '1299018520.09dxschafe': {
                    'Type': 'Collision Barrier',
                    'Bitmasks': [
                        'BattleAimOccludeBitmask'],
                    'DisableCollision': False,
                    'Holiday': '',
                    'Hpr': VBase3(162.480, 0.0, 0.0),
                    'Pos': Point3(-28.806, -26.097, -0.72398),
                    'Scale': VBase3(1.831, 1.831, 3.8479),
                    'VisSize': '',
                    'Visual': {
                        'Model': 'models/misc/pir_m_prp_lev_cambarrier_plane' } } },
            'Visibility': 'Grid',
            'Visual': {
                'Model': 'models/jungles/jungle_a_zero' } } },
    'Node Links': [],
    'Layers': { },
    'ObjectIds': {
        '1170793088.0jubutler': '["Objects"]["1170793088.0jubutler"]',
        '1170793216.0jubutler': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170793216.0jubutler"]',
        '1170793216.0jubutler0': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170793216.0jubutler0"]',
        '1170793216.0jubutler1': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170793216.0jubutler1"]',
        '1170913647.47darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913647.47darren"]',
        '1170913737.7darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913737.7darren"]',
        '1170913749.09darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913749.09darren"]',
        '1170913790.58darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913790.58darren"]',
        '1170913808.86darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913808.86darren"]',
        '1170913819.52darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913819.52darren"]',
        '1170913839.28darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170913839.28darren"]',
        '1170978980.97darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1170978980.97darren"]',
        '1171402341.13darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1171402341.13darren"]',
        '1171402396.28darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1171402396.28darren"]',
        '1171402484.72darren': '["Objects"]["1170793088.0jubutler"]["Objects"]["1171402484.72darren"]',
        '1182814104.67kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182814104.67kmuller"]',
        '1182878962.0kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182878962.0kmuller"]',
        '1182879030.06kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879030.06kmuller"]',
        '1182879074.77kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879074.77kmuller"]',
        '1182879141.03kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879141.03kmuller"]',
        '1182879273.41kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879273.41kmuller"]',
        '1182879325.2kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879325.2kmuller"]',
        '1182879369.95kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879369.95kmuller"]',
        '1182879410.14kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182879410.14kmuller"]',
        '1182880772.45kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182880772.45kmuller"]',
        '1182881679.11kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182881679.11kmuller"]',
        '1182881711.8kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182881711.8kmuller"]',
        '1182881836.17kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182881836.17kmuller"]',
        '1182881912.22kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182881912.22kmuller"]',
        '1182881939.48kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182881939.48kmuller"]',
        '1182882005.38kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882005.38kmuller"]',
        '1182882019.13kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882019.13kmuller"]',
        '1182882095.56kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882095.56kmuller"]',
        '1182882150.58kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882150.58kmuller"]',
        '1182882170.84kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882170.84kmuller"]',
        '1182882199.61kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882199.61kmuller"]',
        '1182882218.36kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882218.36kmuller"]',
        '1182882232.19kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882232.19kmuller"]',
        '1182882314.53kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882314.53kmuller"]',
        '1182882445.39kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882445.39kmuller"]',
        '1182882526.47kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882526.47kmuller"]',
        '1182882605.27kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882605.27kmuller"]',
        '1182882633.61kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882633.61kmuller"]',
        '1182882820.56kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882820.56kmuller"]',
        '1182882887.55kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882887.55kmuller"]',
        '1182882922.02kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882922.02kmuller"]',
        '1182882948.95kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182882948.95kmuller"]',
        '1182883004.3kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883004.3kmuller"]',
        '1182883034.56kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883034.56kmuller"]',
        '1182883082.84kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883082.84kmuller"]',
        '1182883114.56kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883114.56kmuller"]',
        '1182883544.25kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883544.25kmuller"]',
        '1182883780.25kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182883780.25kmuller"]',
        '1182893238.66kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893238.66kmuller"]',
        '1182893279.69kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893279.69kmuller"]',
        '1182893305.72kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893305.72kmuller"]',
        '1182893330.14kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893330.14kmuller"]',
        '1182893343.03kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893343.03kmuller"]',
        '1182893391.86kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893391.86kmuller"]',
        '1182893473.22kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893473.22kmuller"]',
        '1182893504.52kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893504.52kmuller"]',
        '1182893516.55kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893516.55kmuller"]',
        '1182893528.22kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893528.22kmuller"]',
        '1182893666.75kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893666.75kmuller"]',
        '1182893701.58kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893701.58kmuller"]',
        '1182893745.98kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893745.98kmuller"]',
        '1182893812.02kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893812.02kmuller"]',
        '1182893835.89kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893835.89kmuller"]',
        '1182893853.69kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182893853.69kmuller"]',
        '1182894165.75kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894165.75kmuller"]',
        '1182894212.08kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894212.08kmuller"]',
        '1182894264.47kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894264.47kmuller"]',
        '1182894369.28kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894369.28kmuller"]',
        '1182894489.5kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894489.5kmuller"]',
        '1182894516.83kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894516.83kmuller"]',
        '1182894619.73kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894619.73kmuller"]',
        '1182894920.72kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894920.72kmuller"]',
        '1182894988.89kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182894988.89kmuller"]',
        '1182895013.33kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895013.33kmuller"]',
        '1182895142.08kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895142.08kmuller"]',
        '1182895181.08kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895181.08kmuller"]',
        '1182895214.06kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895214.06kmuller"]',
        '1182895366.86kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895366.86kmuller"]',
        '1182895388.58kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895388.58kmuller"]',
        '1182895463.88kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895463.88kmuller"]',
        '1182895495.86kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895495.86kmuller"]',
        '1182895526.56kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895526.56kmuller"]',
        '1182895565.48kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895565.48kmuller"]',
        '1182895578.3kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895578.3kmuller"]',
        '1182895794.27kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895794.27kmuller"]',
        '1182895831.91kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895831.91kmuller"]',
        '1182895912.91kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182895912.91kmuller"]',
        '1182896654.44kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896654.44kmuller"]',
        '1182896712.94kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896712.94kmuller"]',
        '1182896730.05kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896730.05kmuller"]',
        '1182896752.06kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896752.06kmuller"]',
        '1182896791.03kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896791.03kmuller"]',
        '1182896838.22kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896838.22kmuller"]',
        '1182896857.69kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896857.69kmuller"]',
        '1182896876.94kmuller': '["Objects"]["1170793088.0jubutler"]["Objects"]["1182896876.94kmuller"]',
        '1296000903.4dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296000903.4dxschafe"]',
        '1296001095.51dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001095.51dxschafe"]',
        '1296001209.79dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001209.79dxschafe"]',
        '1296001473.01dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001473.01dxschafe"]',
        '1296001494.81dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001494.81dxschafe"]',
        '1296001873.23dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001873.23dxschafe"]',
        '1296001996.47dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296001996.47dxschafe"]',
        '1296002311.4dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296002311.4dxschafe"]',
        '1296002499.26dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296002499.26dxschafe"]',
        '1296002574.36dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296002574.36dxschafe"]',
        '1296003330.89dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296003330.89dxschafe"]',
        '1296003457.09dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296003457.09dxschafe"]',
        '1296003522.06dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296003522.06dxschafe"]',
        '1296003753.53dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296003753.53dxschafe"]',
        '1296003804.4dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1296003804.4dxschafe"]',
        '1299005027.93dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005027.93dxschafe"]',
        '1299005184.47dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005184.47dxschafe"]',
        '1299005251.06dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005251.06dxschafe"]',
        '1299005299.73dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005299.73dxschafe"]',
        '1299005356.8dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005356.8dxschafe"]',
        '1299005406.16dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005406.16dxschafe"]',
        '1299005463.63dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005463.63dxschafe"]',
        '1299005504.14dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005504.14dxschafe"]',
        '1299005587.34dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005587.34dxschafe"]',
        '1299005766.98dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005766.98dxschafe"]',
        '1299005796.15dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005796.15dxschafe"]',
        '1299005829.18dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005829.18dxschafe"]',
        '1299005876.92dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005876.92dxschafe"]',
        '1299005929.28dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005929.28dxschafe"]',
        '1299005959.3dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299005959.3dxschafe"]',
        '1299006015.54dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006015.54dxschafe"]',
        '1299006066.12dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006066.12dxschafe"]',
        '1299006151.94dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006151.94dxschafe"]',
        '1299006424.19dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006424.19dxschafe"]',
        '1299006444.35dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006444.35dxschafe"]',
        '1299006481.68dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006481.68dxschafe"]',
        '1299006908.84dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299006908.84dxschafe"]',
        '1299007243.23dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299007243.23dxschafe"]',
        '1299007328.79dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299007328.79dxschafe"]',
        '1299008023.1dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299008023.1dxschafe"]',
        '1299008069.45dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299008069.45dxschafe"]',
        '1299008172.7dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299008172.7dxschafe"]',
        '1299008387.87dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299008387.87dxschafe"]',
        '1299008427.65dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299008427.65dxschafe"]',
        '1299015944.45dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299015944.45dxschafe"]',
        '1299016041.0dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016041.0dxschafe"]',
        '1299016096.64dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016096.64dxschafe"]',
        '1299016135.41dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016135.41dxschafe"]',
        '1299016198.17dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016198.17dxschafe"]',
        '1299016245.59dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016245.59dxschafe"]',
        '1299016287.23dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016287.23dxschafe"]',
        '1299016345.28dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016345.28dxschafe"]',
        '1299016436.66dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016436.66dxschafe"]',
        '1299016482.6dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016482.6dxschafe"]',
        '1299016541.41dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016541.41dxschafe"]',
        '1299016586.49dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016586.49dxschafe"]',
        '1299016643.53dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016643.53dxschafe"]',
        '1299016772.47dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016772.47dxschafe"]',
        '1299016822.15dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016822.15dxschafe"]',
        '1299016871.9dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016871.9dxschafe"]',
        '1299016935.79dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016935.79dxschafe"]',
        '1299016970.52dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299016970.52dxschafe"]',
        '1299017032.85dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017032.85dxschafe"]',
        '1299017165.26dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017165.26dxschafe"]',
        '1299017212.52dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017212.52dxschafe"]',
        '1299017256.68dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017256.68dxschafe"]',
        '1299017293.23dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017293.23dxschafe"]',
        '1299017345.73dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017345.73dxschafe"]',
        '1299017391.34dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017391.34dxschafe"]',
        '1299017434.99dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017434.99dxschafe"]',
        '1299017467.95dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017467.95dxschafe"]',
        '1299017522.01dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017522.01dxschafe"]',
        '1299017572.31dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017572.31dxschafe"]',
        '1299017821.85dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017821.85dxschafe"]',
        '1299017857.85dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017857.85dxschafe"]',
        '1299017878.48dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017878.48dxschafe"]',
        '1299017913.86dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017913.86dxschafe"]',
        '1299017969.0dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299017969.0dxschafe"]',
        '1299018020.76dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018020.76dxschafe"]',
        '1299018064.79dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018064.79dxschafe"]',
        '1299018104.2dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018104.2dxschafe"]',
        '1299018135.89dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018135.89dxschafe"]',
        '1299018159.23dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018159.23dxschafe"]',
        '1299018201.33dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018201.33dxschafe"]',
        '1299018227.75dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018227.75dxschafe"]',
        '1299018259.9dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018259.9dxschafe"]',
        '1299018293.18dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018293.18dxschafe"]',
        '1299018345.51dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018345.51dxschafe"]',
        '1299018375.08dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018375.08dxschafe"]',
        '1299018411.16dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018411.16dxschafe"]',
        '1299018445.2dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018445.2dxschafe"]',
        '1299018490.56dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018490.56dxschafe"]',
        '1299018520.09dxschafe': '["Objects"]["1170793088.0jubutler"]["Objects"]["1299018520.09dxschafe"]' } }
extraInfo = {
    'camPos': Point3(-344.377, 383.519, 109.197),
    'camHpr': VBase3(-115.307, 0.402108, 0),
    'focalLength': 0.85199999809299998,
    'skyState': 6,
    'fog': 0 }
