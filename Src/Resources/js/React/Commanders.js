import React, { Component } from 'react';
import { render } from 'react-dom';
import { UserRow } from './Components/CommandersChildren';


const baseUri = "/admin/commanders";

export default class UserManagement extends Component {
    constructor(props) {
        super(props);

        this.state = {
            users: null,
            roles: null,
            userIndex: null
        }
    }

    componentDidMount() { this.getData(); }

    //Uri: Roles - Gets all of the roles that can be assigned to accounts
    //Uri: Elevated - Gets a list of all users with one or more account roles
    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/elevated`,
        }).done((result) => {
            this.setState({ users: result });
        }).fail((err) => {
            console.error(`React/Commanders {Commanders@getData} - Error getting active users`, err);
        });

        $.ajax({
            type: 'get',
            url: `${baseUri}/roles`,
        }).done((result) => {
            this.setState({ roles: result });
        }).fail((err) => {
            console.error(`React/Commanders {Commanders@getData} - Error getting the avaliable acount roles`, err);
        });

        this.setUserIndex(null);
    }

    setUserIndex(i) {
        this.setState({ userIndex: i });
    }

    getUsers() {
        return (!!this.state.users) ? this.state.users : null;
    }


    render() {
        let users;
        if (!!this.getUsers()) {
            users = this.getUsers().map((user, index) => {
                return <UserRow user={user} viewDetails={this.setUserIndex.bind(this)} index={index} key={index} />
            });
        }
        let userDetails;

        return (
            <div className="container">
                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <table className="table table-responsive">
                            <thead>
                                <tr className="font-alpha">
                                    <th></th>
                                    <th>Name</th>
                                    <th>Corporation</th>
                                    <th>Alliance</th>
                                    <th>Roles</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {users}
                            </tbody>
                        </table>
                    </div>

                    <div className="col-lg-4 col-md-12">
                        {userDetails}
                    </div>
                </div>
            </div>
        )
    }
}



if (document.getElementById('user_management'))
    render(<UserManagement />, document.getElementById('user_management'));