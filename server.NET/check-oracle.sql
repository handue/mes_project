-- Oracle DB 데이터 확인 스크립트
-- Oracle DB Data Check Script

-- 실행 방법: docker exec -i oracle-mes sqlplus mes/mes123@//localhost:1521/XE < check-oracle.sql
-- How to run: docker exec -i oracle-mes sqlplus mes/mes123@//localhost:1521/XE < check-oracle.sql

-- 1. 테이블 목록 확인
-- 1. Check table list
SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME;

-- 2. WORKORDER 테이블 데이터 확인
-- 2. Check WORKORDER table data count
SELECT COUNT(*) as WORKORDER_COUNT FROM WORKORDER;

-- 3. MACHINE 테이블 데이터 확인  
-- 3. Check MACHINE table data count
SELECT COUNT(*) as MACHINE_COUNT FROM MACHINE;

-- 4. INVENTORY 테이블 데이터 확인
-- 4. Check INVENTORY table data count
SELECT COUNT(*) as INVENTORY_COUNT FROM INVENTORY;

-- 5. QUALITYCONTROL 테이블 데이터 확인
-- 5. Check QUALITYCONTROL table data count
SELECT COUNT(*) as QUALITYCONTROL_COUNT FROM QUALITYCONTROL;

-- 6. 샘플 데이터 확인 (WORKORDER)
-- 6. Sample data from WORKORDER
SELECT ORDERID, PRODUCTID, STATUS, QUANTITY FROM WORKORDER WHERE ROWNUM <= 5;

-- 7. 샘플 데이터 확인 (MACHINE)
-- 7. Sample data from MACHINE
SELECT MACHINEID, NAME, STATUS, TYPE FROM MACHINE WHERE ROWNUM <= 5;

-- 8. 샘플 데이터 확인 (INVENTORY)
-- 8. Sample data from INVENTORY
SELECT ITEMID, NAME, QUANTITY, COST FROM INVENTORY WHERE ROWNUM <= 5;

-- 9. 테이블 구조 확인
-- 9. Check table structure
DESC WORKORDER;
DESC MACHINE;
DESC INVENTORY;
DESC QUALITYCONTROL;

-- 10. 사용자 정보 확인
-- 10. Check user info
SELECT USER, SYSDATE FROM DUAL; 