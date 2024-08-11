import csv
import random
import uuid
from faker import Faker

# Initialize Faker for generating random data
faker = Faker()

# List of genders
genders = ['Male', 'Female']

# Generate random student data
students_data = []
for _ in range(15):
    student = {
        "Id": str(uuid.uuid4()),
        "StudentId": faker.random_number(digits=8, fix_len=True),
        "Name": faker.name(),
        "Username": faker.user_name(),
        "Password": faker.password(length=10),
        "Email": faker.email(),
        "Gender": random.choice(genders),
        "DateOfBirth": faker.date_of_birth(minimum_age=17, maximum_age=23),
        "Address": faker.address().replace("\n", ", "),
        "Phone": faker.phone_number()
    }
    students_data.append(student)

# Define the CSV file path
csv_file_path = 'students_info.csv'

# Write to CSV file
with open(csv_file_path, mode='w', newline='') as file:
    writer = csv.DictWriter(file, fieldnames=students_data[0].keys())
    writer.writeheader()
    for student in students_data:
        writer.writerow(student)

csv_file_path
