USE DL_OTO;
GO

UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/ac/2018_Toyota_Camry_%28ASV70R%29_Ascent_sedan_%282018-08-27%29_01.jpg/500px-2018_Toyota_Camry_%28ASV70R%29_Ascent_sedan_%282018-08-27%29_01.jpg' WHERE MaXe = 'XE01';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a3/Toyota_Vios_1.5_VVT-i_G_%28IV%29_%E2%80%93_f_13032025.jpg/500px-Toyota_Vios_1.5_VVT-i_G_%28IV%29_%E2%80%93_f_13032025.jpg' WHERE MaXe = 'XE02';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/1/1a/Honda_Civic_e-HEV_Sport_%28XI%29_%E2%80%93_f_30062024.jpg/500px-Honda_Civic_e-HEV_Sport_%28XI%29_%E2%80%93_f_30062024.jpg' WHERE MaXe = 'XE03';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/1/1b/Honda_CR-V_e-HEV_Elegance_AWD_%28VI%29_%E2%80%93_f_14072024.jpg/500px-Honda_CR-V_e-HEV_Elegance_AWD_%28VI%29_%E2%80%93_f_14072024.jpg' WHERE MaXe = 'XE04';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/a5/2024_Mazda_CX-5_2.5_S_Select_in_Platinum_Quartz_Metallic%2C_front_right.jpg/500px-2024_Mazda_CX-5_2.5_S_Select_in_Platinum_Quartz_Metallic%2C_front_right.jpg' WHERE MaXe = 'XE05';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/4/41/Hyundai_Accent_1.5_MPI_Smart%2B_%28VI%29_%E2%80%93_f_08032025.jpg/500px-Hyundai_Accent_1.5_MPI_Smart%2B_%28VI%29_%E2%80%93_f_08032025.jpg' WHERE MaXe = 'XE06';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/8/84/2024_Kia_Sorento_X-Line_SX_Prestige_%28facelift%29%2C_front_12.20.24.jpg/500px-2024_Kia_Sorento_X-Line_SX_Prestige_%28facelift%29%2C_front_12.20.24.jpg' WHERE MaXe = 'XE07';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/ab/Ford_Everest_3.0_V6_Turbo_Diesel_4WD_Platinum_%28III%29_%E2%80%93_f_02012026.jpg/500px-Ford_Everest_3.0_V6_Turbo_Diesel_4WD_Platinum_%28III%29_%E2%80%93_f_02012026.jpg' WHERE MaXe = 'XE08';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/b/be/Mercedes-Benz_W206_IMG_6380.jpg/500px-Mercedes-Benz_W206_IMG_6380.jpg' WHERE MaXe = 'XE09';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/f/f1/2019_BMW_X5_M50d_Automatic_3.0.jpg/500px-2019_BMW_X5_M50d_Automatic_3.0.jpg' WHERE MaXe = 'XE10';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/6/61/VinFast_VF_8_DSC_8568.jpg/500px-VinFast_VF_8_DSC_8568.jpg' WHERE MaXe = 'XE11';
UPDATE Xe SET HinhAnh = 'https://upload.wikimedia.org/wikipedia/commons/thumb/a/ab/Tesla_Model_3_%282023%29_Autofr%C3%BChling_Ulm_IMG_9282.jpg/500px-Tesla_Model_3_%282023%29_Autofr%C3%BChling_Ulm_IMG_9282.jpg' WHERE MaXe = 'XE12';
GO

SELECT MaXe, TenXe, HinhAnh FROM Xe ORDER BY MaXe;
GO
